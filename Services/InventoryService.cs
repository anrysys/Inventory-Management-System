using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Models;

namespace InventorySystem.Services
{
    /// <summary>
    /// Provides business logic for managing product inventory operations.
    /// </summary>
    public class InventoryService
    {
        private readonly List<Product> _products;
        private int _nextId;

        /// <summary>
        /// Initializes a new instance of the InventoryService class.
        /// </summary>
        public InventoryService()
        {
            _products = new List<Product>();
            _nextId = 1;
        }

        /// <summary>
        /// Gets the total number of products in the inventory.
        /// </summary>
        public int ProductCount => _products.Count;

        /// <summary>
        /// Adds a new product to the inventory.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="quantity">The initial quantity of the product.</param>
        /// <returns>The newly created product.</returns>
        /// <exception cref="ArgumentException">Thrown when a product with the same name already exists.</exception>
        public Product AddProduct(string name, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be null or empty.", nameof(name));

            if (price <= 0)
                throw new ArgumentException("Product price must be greater than zero.", nameof(price));

            if (quantity < 0)
                throw new ArgumentException("Product quantity cannot be negative.", nameof(quantity));

            // Check for duplicate product names (case-insensitive)
            if (_products.Any(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"A product with the name '{name}' already exists.", nameof(name));
            }

            var product = new Product(_nextId++, name, price, quantity);
            _products.Add(product);

            return product;
        }

        /// <summary>
        /// Updates the quantity of an existing product.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="newQuantity">The new quantity for the product.</param>
        /// <returns>The updated product.</returns>
        /// <exception cref="ArgumentException">Thrown when the product is not found or quantity is invalid.</exception>
        public Product UpdateProductQuantity(int productId, int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Product quantity cannot be negative.", nameof(newQuantity));

            var product = FindProductById(productId);
            if (product == null)
                throw new ArgumentException($"Product with ID {productId} not found.", nameof(productId));

            product.Quantity = newQuantity;
            return product;
        }

        /// <summary>
        /// Retrieves all products in the inventory.
        /// </summary>
        /// <returns>A list of all products.</returns>
        public List<Product> GetAllProducts()
        {
            return new List<Product>(_products);
        }

        /// <summary>
        /// Deletes a product from the inventory.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <returns>True if the product was successfully deleted; otherwise, false.</returns>
        public bool DeleteProduct(int productId)
        {
            var product = FindProductById(productId);
            if (product == null)
                return false;

            return _products.Remove(product);
        }

        /// <summary>
        /// Finds a product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to find.</param>
        /// <returns>The product if found; otherwise, null.</returns>
        public Product? FindProductById(int productId)
        {
            return _products.FirstOrDefault(p => p.Id == productId);
        }

        /// <summary>
        /// Finds products by name (case-insensitive partial match).
        /// </summary>
        /// <param name="name">The name or partial name to search for.</param>
        /// <returns>A list of matching products.</returns>
        public List<Product> FindProductsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Product>();

            return _products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                           .ToList();
        }

        /// <summary>
        /// Gets products with low stock (quantity below the specified threshold).
        /// </summary>
        /// <param name="threshold">The stock threshold (default is 5).</param>
        /// <returns>A list of products with low stock.</returns>
        public List<Product> GetLowStockProducts(int threshold = 5)
        {
            return _products.Where(p => p.Quantity < threshold).ToList();
        }

        /// <summary>
        /// Gets the total value of all inventory.
        /// </summary>
        /// <returns>The total value of inventory (price × quantity for all products).</returns>
        public decimal GetTotalInventoryValue()
        {
            return _products.Sum(p => p.Price * p.Quantity);
        }

        /// <summary>
        /// Checks if the inventory is empty.
        /// </summary>
        /// <returns>True if the inventory is empty; otherwise, false.</returns>
        public bool IsInventoryEmpty()
        {
            return _products.Count == 0;
        }

        /// <summary>
        /// Generates the next available product ID.
        /// </summary>
        /// <returns>The next available ID.</returns>
        public int GenerateNextId()
        {
            return _nextId;
        }

        /// <summary>
        /// Gets a formatted string representation of the inventory summary.
        /// </summary>
        /// <returns>A formatted summary of the inventory.</returns>
        public string GetInventorySummary()
        {
            if (IsInventoryEmpty())
            {
                return "Inventory is empty.";
            }

            var summary = $"=== Inventory Summary ===\n";
            summary += $"Total Products: {ProductCount}\n";
            summary += $"Total Inventory Value: ${GetTotalInventoryValue():F2}\n";

            var lowStockItems = GetLowStockProducts();
            if (lowStockItems.Any())
            {
                summary += $"Low Stock Items ({lowStockItems.Count}): ";
                summary += string.Join(", ", lowStockItems.Select(p => p.Name));
            }

            return summary;
        }
    }
}
