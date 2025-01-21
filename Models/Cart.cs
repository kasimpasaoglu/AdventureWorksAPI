public class ShoppingCartDTO
{
    public int TransactionId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal ListPrice { get; set; }
    public decimal TotalPrice => ListPrice * Quantity;
    public DateTime DateCreated { get; set; }
    public DateTime ModifiedDate { get; set; }
}


public class InsertVM
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
