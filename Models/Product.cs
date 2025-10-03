using System;

namespace InventorySystem.Models
{
    /// <summary>
    /// Represents a product in the inventory system with basic properties and methods.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the unique identifier for the product.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product in stock.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Initializes a new instance of the Product class.
        /// </summary>
        public Product()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Product class with specified values.
        /// </summary>
        /// <param name="id">The unique identifier for the product.</param>
        /// <param name="name">The name of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="quantity">The quantity of the product in stock.</param>
        public Product(int id, string name, decimal price, int quantity)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Price = price;
            Quantity = quantity;
        }

        /// <summary>
        /// Returns a string representation of the product.
        /// </summary>
        /// <returns>A formatted string containing product information.</returns>
        public override string ToString()
        {
            return $"ID: {Id} | Name: {Name} | Price: ${Price:F2} | Quantity: {Quantity}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current product.
        /// </summary>
        /// <param name="obj">The object to compare with the current product.</param>
        /// <returns>True if the specified object is equal to the current product; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Product other)
            {
                return Id == other.Id;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this product.
        /// </summary>
        /// <returns>A hash code for the current product.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
