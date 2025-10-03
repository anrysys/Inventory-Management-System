using System;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.Utils;

namespace InventorySystem
{
    /// <summary>
    /// Main program class for the Inventory Management System.
    /// </summary>
    class Program
    {
        private static InventoryService _inventoryService = new InventoryService();

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Inventory Management System!");
            Console.WriteLine("==========================================");

            // Main application loop
            bool isRunning = true;
            while (isRunning)
            {
                try
                {
                    DisplayMenu();
                    var choice = GetMenuChoice();
                    isRunning = ProcessUserChoice(choice);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    if (Console.IsInputRedirected == false)
                    {
                        Console.ReadKey();
                    }
                }
            }

            Console.WriteLine("Thank you for using the Inventory Management System!");
            
            // Only wait for key press if console input is available
            if (Console.IsInputRedirected == false)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the main menu options to the user.
        /// </summary>
        private static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Inventory Management System ===");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Update Inventory");
            Console.WriteLine("3. View All Products");
            Console.WriteLine("4. Delete Product");
            Console.WriteLine("5. Exit");
            Console.WriteLine("====================================");
            Console.WriteLine(_inventoryService.GetInventorySummary());
            Console.WriteLine("====================================");
        }

        /// <summary>
        /// Gets and validates the user's menu choice.
        /// </summary>
        /// <returns>The selected menu option.</returns>
        private static MenuOption GetMenuChoice()
        {
            while (true)
            {
                Console.Write("Enter your choice (1-5): ");
                string? input = Console.ReadLine();

                if (InputValidator.ValidateMenuChoice(input ?? string.Empty, out MenuOption choice))
                {
                    return choice;
                }

                Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
            }
        }

        /// <summary>
        /// Processes the user's menu choice and executes the corresponding action.
        /// </summary>
        /// <param name="choice">The menu option selected by the user.</param>
        /// <returns>True to continue running the application; false to exit.</returns>
        private static bool ProcessUserChoice(MenuOption choice)
        {
            switch (choice)
            {
                case MenuOption.AddProduct:
                    AddProductFlow();
                    break;

                case MenuOption.UpdateInventory:
                    UpdateInventoryFlow();
                    break;

                case MenuOption.ViewProducts:
                    ViewProductsFlow();
                    break;

                case MenuOption.DeleteProduct:
                    DeleteProductFlow();
                    break;

                case MenuOption.Exit:
                    return false;

                default:
                    Console.WriteLine("Invalid option selected.");
                    break;
            }

            if (choice != MenuOption.Exit)
            {
                Console.WriteLine("\nPress any key to continue...");
                if (Console.IsInputRedirected == false)
                {
                    Console.ReadKey();
                }
            }

            return true;
        }

        /// <summary>
        /// Handles the flow for adding a new product.
        /// </summary>
        private static void AddProductFlow()
        {
            Console.WriteLine("\n=== Add New Product ===");

            try
            {
                // Get product name
                string name = InputValidator.GetValidatedInput<string>(
                    "Enter product name: ",
                    input => (InputValidator.ValidateProductName(input), input),
                    "Product name must be at least 2 characters long and not empty."
                );

                // Get product price
                decimal price = InputValidator.GetValidatedInput<decimal>(
                    "Enter product price: $",
                    input => {
                        bool isValid = InputValidator.ValidatePrice(input, out decimal parsedPrice);
                        return (isValid, parsedPrice);
                    },
                    "Price must be a positive number."
                );

                // Get product quantity
                int quantity = InputValidator.GetValidatedInput<int>(
                    "Enter product quantity: ",
                    input => {
                        bool isValid = InputValidator.ValidateQuantity(input, out int parsedQuantity);
                        return (isValid, parsedQuantity);
                    },
                    "Quantity must be a non-negative integer."
                );

                // Add the product
                var product = _inventoryService.AddProduct(name, price, quantity);
                Console.WriteLine($"\n✓ Product '{product.Name}' added successfully!");
                Console.WriteLine($"Product Details: {product}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("\n✗ Operation cancelled due to invalid input.");
            }
        }

        /// <summary>
        /// Handles the flow for updating product inventory.
        /// </summary>
        private static void UpdateInventoryFlow()
        {
            Console.WriteLine("\n=== Update Product Inventory ===");

            if (_inventoryService.IsInventoryEmpty())
            {
                Console.WriteLine("No products available to update.");
                return;
            }

            try
            {
                // Display current products
                DisplayCurrentProducts();

                // Get product ID
                int productId = InputValidator.GetValidatedInput<int>(
                    "Enter product ID to update: ",
                    input => {
                        bool isValid = InputValidator.ValidateProductId(input, out int parsedId);
                        return (isValid, parsedId);
                    },
                    "Product ID must be a positive integer."
                );

                // Check if product exists
                var existingProduct = _inventoryService.FindProductById(productId);
                if (existingProduct == null)
                {
                    Console.WriteLine($"✗ Product with ID {productId} not found.");
                    return;
                }

                Console.WriteLine($"Current product: {existingProduct}");

                // Get new quantity
                int newQuantity = InputValidator.GetValidatedInput<int>(
                    "Enter new quantity: ",
                    input => {
                        bool isValid = InputValidator.ValidateQuantity(input, out int parsedQuantity);
                        return (isValid, parsedQuantity);
                    },
                    "Quantity must be a non-negative integer."
                );

                // Update the product
                var updatedProduct = _inventoryService.UpdateProductQuantity(productId, newQuantity);
                Console.WriteLine($"\n✓ Product '{updatedProduct.Name}' quantity updated to {newQuantity} successfully!");
                Console.WriteLine($"Updated Details: {updatedProduct}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("\n✗ Operation cancelled due to invalid input.");
            }
        }

        /// <summary>
        /// Handles the flow for viewing all products.
        /// </summary>
        private static void ViewProductsFlow()
        {
            Console.WriteLine("\n=== Current Inventory ===");

            var products = _inventoryService.GetAllProducts();

            if (products.Count == 0)
            {
                Console.WriteLine("No products in inventory.");
                return;
            }

            // Display products in a formatted table
            Console.WriteLine($"{"ID",-4} | {"Name",-20} | {"Price",-10} | {"Quantity",-8}");
            Console.WriteLine(new string('-', 50));

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                Console.WriteLine($"{product.Id,-4} | {product.Name,-20} | ${product.Price,-9:F2} | {product.Quantity,-8}");
            }

            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Total Products: {products.Count}");
            Console.WriteLine($"Total Inventory Value: ${_inventoryService.GetTotalInventoryValue():F2}");

            // Show low stock warning
            var lowStockProducts = _inventoryService.GetLowStockProducts();
            if (lowStockProducts.Count > 0)
            {
                Console.WriteLine("\n⚠️  Low Stock Alert:");
                foreach (var product in lowStockProducts)
                {
                    Console.WriteLine($"   - {product.Name} (Quantity: {product.Quantity})");
                }
            }
        }

        /// <summary>
        /// Handles the flow for deleting a product.
        /// </summary>
        private static void DeleteProductFlow()
        {
            Console.WriteLine("\n=== Delete Product ===");

            if (_inventoryService.IsInventoryEmpty())
            {
                Console.WriteLine("No products available to delete.");
                return;
            }

            try
            {
                // Display current products
                DisplayCurrentProducts();

                // Get product ID
                int productId = InputValidator.GetValidatedInput<int>(
                    "Enter product ID to delete: ",
                    input => {
                        bool isValid = InputValidator.ValidateProductId(input, out int parsedId);
                        return (isValid, parsedId);
                    },
                    "Product ID must be a positive integer."
                );

                // Check if product exists
                var productToDelete = _inventoryService.FindProductById(productId);
                if (productToDelete == null)
                {
                    Console.WriteLine($"✗ Product with ID {productId} not found.");
                    return;
                }

                Console.WriteLine($"Product to delete: {productToDelete}");

                // Confirm deletion
                bool confirmed = InputValidator.GetConfirmation("Are you sure you want to delete this product?");

                if (confirmed)
                {
                    bool deleted = _inventoryService.DeleteProduct(productId);
                    if (deleted)
                    {
                        Console.WriteLine($"\n✓ Product '{productToDelete.Name}' deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine($"\n✗ Failed to delete product '{productToDelete.Name}'.");
                    }
                }
                else
                {
                    Console.WriteLine("\n❌ Deletion cancelled.");
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("\n✗ Operation cancelled due to invalid input.");
            }
        }

        /// <summary>
        /// Displays a compact list of current products for selection purposes.
        /// </summary>
        private static void DisplayCurrentProducts()
        {
            var products = _inventoryService.GetAllProducts();
            Console.WriteLine("\nCurrent Products:");

            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"  {products[i].Id}. {products[i].Name} (Qty: {products[i].Quantity})");
            }
            Console.WriteLine();
        }
    }
}
