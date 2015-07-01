Feature: Specify which type of organisation I work for

Scenario: Sole trader or individual
Given I selected the sole trader or individual option
When I continue
Then I should be redirected to the sole trader or individual page

Scenario: Partnership
Given I selected the partnership option
When I continue
Then I should be redirected to the partnership details page

Scenario: Registered company
Given I selected the registered company option
When I continue
Then I should be redirected to the registered company details page
