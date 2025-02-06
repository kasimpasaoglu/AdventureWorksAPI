public class ShoppingCartDTO
{
    public Summary Details { get; set; }
    public List<ItemDTO> Items { get; set; }


    public class ItemDTO
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public byte[]? LargePhoto { get; set; }
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
        public decimal TotalPrice => ListPrice * Quantity;
        public DateTime DateCreated { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}

public class ShoppingCartVM
{
    public Summary Details { get; set; }
    public List<ItemVM> Items { get; set; }


    public class ItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public byte[]? LargePhoto { get; set; }
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
        public decimal TotalPrice => ListPrice * Quantity;
    }
}

public class CartItemVM
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class Summary
{
    public decimal TotalPrice { get; set; }
    public int ItemCount { get; set; }
}
