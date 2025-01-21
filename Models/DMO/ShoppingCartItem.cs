using System;
using System.Collections.Generic;

namespace AdventureWorksAPI.Models.DMO;

/// <summary>
/// Contains online customer orders until the order is submitted or cancelled.
/// </summary>
public partial class ShoppingCartItem
{
    /// <summary>
    /// Shopping cart identification number.
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// Product quantity ordered.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Product ordered. Foreign key to Product.ProductID.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Date the time the record was created.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public int BusinessEntityId { get; set; }

    public virtual BusinessEntity BusinessEntity { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
