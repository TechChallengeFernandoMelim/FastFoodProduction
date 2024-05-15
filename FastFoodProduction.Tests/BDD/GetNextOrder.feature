Feature: GetNextOrder
    As a restaurant employee
    I want to be able to retrieve the next order
    So that I can process it

Scenario: Successfully retrieve the next order
    Given there is a valid order available in the messaging service
    When an employee attempts to retrieve the next order
    Then the system should return the order details
    And the order status should indicate it's paid