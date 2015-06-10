Feature: Specify Organisation Details

Scenario: Sole trader or individual details
Given I am a sole trader or individual
When I submit details about my sole trader organisation
Then the details should be stored
And I should be redirected to the select organisation page

Scenario: Partnership details
Given I am a partnership
When I submit details about my partnership organisation
Then the details should be stored
And I should be redirected to the select organisation page

Scenario: Registered company details
Given I am a registered company
When I submit details about my registered company organisation
Then the details should be stored
And I should be redirected to the select organisation page


