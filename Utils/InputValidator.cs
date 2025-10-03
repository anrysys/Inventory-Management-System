using System;
using System.Globalization;

namespace InventorySystem.Utils
{
    /// <summary>
    /// Provides methods for validating user input in the inventory management system.
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Validates that a product name is not null, empty, or whitespace.
        /// </summary>
        /// <param name="name">The product name to validate.</param>
        /// <returns>True if the name is valid; otherwise, false.</returns>
        public static bool ValidateProductName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length >= 2;
        }

        /// <summary>
        /// Validates and parses a price string.
        /// </summary>
        /// <param name="priceInput">The price string to validate and parse.</param>
        /// <param name="price">When this method returns, contains the parsed price if successful; otherwise, 0.</param>
        /// <returns>True if the price is valid and greater than 0; otherwise, false.</returns>
        public static bool ValidatePrice(string priceInput, out decimal price)
        {
            price = 0;

            if (string.IsNullOrWhiteSpace(priceInput))
                return false;

            if (decimal.TryParse(priceInput, NumberStyles.Currency | NumberStyles.Number,
                               CultureInfo.InvariantCulture, out price))
            {
                return price > 0;
            }

            return false;
        }

        /// <summary>
        /// Validates and parses a quantity string.
        /// </summary>
        /// <param name="quantityInput">The quantity string to validate and parse.</param>
        /// <param name="quantity">When this method returns, contains the parsed quantity if successful; otherwise, 0.</param>
        /// <returns>True if the quantity is valid and non-negative; otherwise, false.</returns>
        public static bool ValidateQuantity(string quantityInput, out int quantity)
        {
            quantity = 0;

            if (string.IsNullOrWhiteSpace(quantityInput))
                return false;

            if (int.TryParse(quantityInput, out quantity))
            {
                return quantity >= 0;
            }

            return false;
        }

        /// <summary>
        /// Validates and parses a menu choice string.
        /// </summary>
        /// <param name="choiceInput">The menu choice string to validate and parse.</param>
        /// <param name="menuOption">When this method returns, contains the parsed menu option if successful.</param>
        /// <returns>True if the choice is valid; otherwise, false.</returns>
        public static bool ValidateMenuChoice(string choiceInput, out MenuOption menuOption)
        {
            menuOption = MenuOption.Exit;

            if (string.IsNullOrWhiteSpace(choiceInput))
                return false;

            if (int.TryParse(choiceInput, out int choice))
            {
                if (Enum.IsDefined(typeof(MenuOption), choice))
                {
                    menuOption = (MenuOption)choice;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates and parses a product ID string.
        /// </summary>
        /// <param name="idInput">The ID string to validate and parse.</param>
        /// <param name="id">When this method returns, contains the parsed ID if successful; otherwise, 0.</param>
        /// <returns>True if the ID is valid and positive; otherwise, false.</returns>
        public static bool ValidateProductId(string idInput, out int id)
        {
            id = 0;

            if (string.IsNullOrWhiteSpace(idInput))
                return false;

            if (int.TryParse(idInput, out id))
            {
                return id > 0;
            }

            return false;
        }

        /// <summary>
        /// Gets validated input from the user with retry logic.
        /// </summary>
        /// <typeparam name="T">The type of the expected input.</typeparam>
        /// <param name="prompt">The prompt to display to the user.</param>
        /// <param name="validator">The validation function.</param>
        /// <param name="errorMessage">The error message to display on invalid input.</param>
        /// <param name="maxAttempts">Maximum number of attempts allowed.</param>
        /// <returns>The validated input value.</returns>
        public static T GetValidatedInput<T>(string prompt, Func<string, (bool isValid, T value)> validator,
                                           string errorMessage, int maxAttempts = 3)
        {
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                var (isValid, value) = validator(input ?? string.Empty);

                if (isValid)
                {
                    return value;
                }

                attempts++;
                Console.WriteLine($"{errorMessage} (Attempt {attempts}/{maxAttempts})");

                if (attempts >= maxAttempts)
                {
                    Console.WriteLine("Maximum attempts reached. Returning to main menu.");
                    throw new InvalidOperationException("Maximum validation attempts exceeded.");
                }
            }

            throw new InvalidOperationException("Validation failed.");
        }

        /// <summary>
        /// Prompts the user for confirmation (Y/N).
        /// </summary>
        /// <param name="message">The confirmation message to display.</param>
        /// <returns>True if the user confirms; otherwise, false.</returns>
        public static bool GetConfirmation(string message)
        {
            while (true)
            {
                Console.Write($"{message} (Y/N): ");
                string? input = Console.ReadLine()?.ToUpperInvariant();

                if (input == "Y" || input == "YES")
                    return true;

                if (input == "N" || input == "NO")
                    return false;

                Console.WriteLine("Please enter Y (Yes) or N (No).");
            }
        }
    }
}
