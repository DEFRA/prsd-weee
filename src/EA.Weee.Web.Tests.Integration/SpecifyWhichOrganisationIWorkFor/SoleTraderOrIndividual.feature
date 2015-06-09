Feature: Specify which organisation I work for

Scenario: Sole trader or individual
Given I select the sole trader or indivdual option
When I select continue
Then I should by redirected to the sole trader or individual page
