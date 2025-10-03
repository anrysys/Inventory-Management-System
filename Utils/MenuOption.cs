namespace InventorySystem.Utils
{
    /// <summary>
    /// Represents the available menu options in the inventory management system.
    /// </summary>
    public enum MenuOption
    {
        /// <summary>
        /// Option to add a new product to the inventory.
        /// </summary>
        AddProduct = 1,

        /// <summary>
        /// Option to update the quantity of an existing product.
        /// </summary>
        UpdateInventory = 2,

        /// <summary>
        /// Option to view all products in the inventory.
        /// </summary>
        ViewProducts = 3,

        /// <summary>
        /// Option to delete a product from the inventory.
        /// </summary>
        DeleteProduct = 4,

        /// <summary>
        /// Option to exit the application.
        /// </summary>
        Exit = 5
    }
}
