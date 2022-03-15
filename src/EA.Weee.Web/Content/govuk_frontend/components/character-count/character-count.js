import '../../vendor/polyfills/Function/prototype/bind'
import '../../vendor/polyfills/Event' // addEventListener and event.target normaliziation
import '../../vendor/polyfills/Element/prototype/classList'

function CharacterCount ($module) {
  this.$module = $module
  this.$textarea = $module.querySelector('.govuk-js-character-count')
  if (this.$textarea) {
    this.$countMessage = document.getElementById(this.$textarea.id + '-info')
  }
}

CharacterCount.prototype.defaults = {
  characterCountAttribute: 'data-maxlength',
  wordCountAttribute: 'data-maxwords'
}

// Initialize component
CharacterCount.prototype.init = function () {
  // Check for module
  var $module = this.$module
  var $textarea = this.$textarea
  var $countMessage = this.$countMessage

  if (!$textarea || !$countMessage) {
    return
  }

  // We move count message right after the field
  // Kept for backwards compatibility
  $textarea.insertAdjacentElement('afterend', $countMessage)

  // Read options set using dataset ('data-' values)
  this.options = this.getDataset($module)

  // Determine the limit attribute (characters or words)
  var countAttribute = this.defaults.characterCountAttribute
  if (this.options.maxwords) {
    countAttribute = this.defaults.wordCountAttribute
  }

  // Save the element limit
  this.maxLength = $module.getAttribute(countAttribute)

  // Check for limit
  if (!this.maxLength) {
    return
  }

  // Remove hard limit if set
  $module.removeAttribute('maxlength')

  // When the page is restored after navigating 'back' in some browsers the
  // state of the character count is not restored until *after* the DOMContentLoaded
  // event is fired, so we need to sync after the pageshow event in browsers
  // that support it.
  if ('onpageshow' in window) {
    window.addEventListener('pageshow', this.sync.bind(this))
  } else {
    window.addEventListener('DOMContentLoaded', this.sync.bind(this))
  }

  this.sync()
}

CharacterCount.prototype.sync = function () {
  this.bindChangeEvents()
  this.updateCountMessage()
}

// Read data attributes
CharacterCount.prototype.getDataset = function (element) {
  var dataset = {}
  var attributes = element.attributes
  if (attributes) {
    for (var i = 0; i < attributes.length; i++) {
      var attribute = attributes[i]
      var match = attribute.name.match(/^data-(.+)/)
      if (match) {
        dataset[match[1]] = attribute.value
      }
    }
  }
  return dataset
}

// Counts characters or words in text
CharacterCount.prototype.count = function (text) {
  var length
  if (this.options.maxwords) {
    var tokens = text.match(/\S+/g) || [] // Matches consecutive non-whitespace chars
    length = tokens.length
  } else {
    length = text.length
  }
  return length
}

// Bind input propertychange to the elements and update based on the change
CharacterCount.prototype.bindChangeEvents = function () {
  var $textarea = this.$textarea
  $textarea.addEventListener('keyup', this.checkIfValueChanged.bind(this))

  // Bind focus/blur events to start/stop polling
  $textarea.addEventListener('focus', this.handleFocus.bind(this))
  $textarea.addEventListener('blur', this.handleBlur.bind(this))
}

// Speech recognition software such as Dragon NaturallySpeaking will modify the
// fields by directly changing its `value`. These changes don't trigger events
// in JavaScript, so we need to poll to handle when and if they occur.
CharacterCount.prototype.checkIfValueChanged = function () {
  if (!this.$textarea.oldValue) this.$textarea.oldValue = ''
  if (this.$textarea.value !== this.$textarea.oldValue) {
    this.$textarea.oldValue = this.$textarea.value
    this.updateCountMessage()
  }
}

// Update message box
CharacterCount.prototype.updateCountMessage = function () {
  var countElement = this.$textarea
  var options = this.options
  var countMessage = this.$countMessage

  // Determine the remaining number of characters/words
  var currentLength = this.count(countElement.value)
  var maxLength = this.maxLength
  var remainingNumber = maxLength - currentLength

  // Set threshold if presented in options
  var thresholdPercent = options.threshold ? options.threshold : 0
  var thresholdValue = maxLength * thresholdPercent / 100
  if (thresholdValue > currentLength) {
    countMessage.classList.add('govuk-character-count__message--disabled')
    // Ensure threshold is hidden for users of assistive technologies
    countMessage.setAttribute('aria-hidden', true)
  } else {
    countMessage.classList.remove('govuk-character-count__message--disabled')
    // Ensure threshold is visible for users of assistive technologies
    countMessage.removeAttribute('aria-hidden')
  }

  // Update styles
  if (remainingNumber < 0) {
    countElement.classList.add('govuk-textarea--error')
    countMessage.classList.remove('govuk-hint')
    countMessage.classList.add('govuk-error-message')
  } else {
    countElement.classList.remove('govuk-textarea--error')
    countMessage.classList.remove('govuk-error-message')
    countMessage.classList.add('govuk-hint')
  }

  // Update message
  var charVerb = 'remaining'
  var charNoun = 'character'
  var displayNumber = remainingNumber
  if (options.maxwords) {
    charNoun = 'word'
  }
  charNoun = charNoun + ((remainingNumber === -1 || remainingNumber === 1) ? '' : 's')

  charVerb = (remainingNumber < 0) ? 'too many' : 'remaining'
  displayNumber = Math.abs(remainingNumber)

  countMessage.innerHTML = 'You have ' + displayNumber + ' ' + charNoun + ' ' + charVerb
}

CharacterCount.prototype.handleFocus = function () {
  // Check if value changed on focus
  this.valueChecker = setInterval(this.checkIfValueChanged.bind(this), 1000)
}

CharacterCount.prototype.handleBlur = function () {
  // Cancel value checking on blur
  clearInterval(this.valueChecker)
}

export default CharacterCount
