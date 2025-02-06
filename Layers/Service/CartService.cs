using AdventureWorksAPI.Models.DMO;

public interface ICartService
{
    Task<bool> InsertNewItemAsync(CartItemVM item, int businessEntityId);
    Task<bool> DeleteItemAsync(CartItemVM item, int businessEntityId);
    Task<ShoppingCartDTO> GetCartItems(int businessEntityId);
}

public class CartService : ICartService
{
    private IUnitOfWork _unitOfWork;
    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> InsertNewItemAsync(CartItemVM item, int businessEntityId)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var isExists = await _unitOfWork.ShoppingCart.FindSingle<ShoppingCartItem>(
                x => x.ProductId == item.ProductId && x.BusinessEntityId == businessEntityId);
            if (isExists == null)
            {
                var cartItem = new ShoppingCartItem()
                {
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    DateCreated = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    BusinessEntityId = businessEntityId,
                };
                await _unitOfWork.ShoppingCart.AddAsync(cartItem);
            }
            else
            {
                isExists.Quantity += item.Quantity;
                isExists.ModifiedDate = DateTime.UtcNow;
                await _unitOfWork.ShoppingCart.UpdateAsync(isExists);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new InvalidOperationException("An error occurred while updating the user. See inner exception for details.", ex);
        }
    }

    public async Task<bool> DeleteItemAsync(CartItemVM item, int businessEntityId)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var cartItem = await _unitOfWork.ShoppingCart.FindSingle<ShoppingCartItem>(
                x => x.ProductId == item.ProductId && x.BusinessEntityId == businessEntityId
            ) ?? throw new ArgumentException($"Item with ProductId {item.ProductId} not found in cart for BusinessEntityId {businessEntityId}");

            cartItem.Quantity = Math.Max(cartItem.Quantity - item.Quantity, 0); // sifirin altina inmesin!

            if (cartItem.Quantity == 0)
            {
                await _unitOfWork.ShoppingCart.RemoveAsync(cartItem);
            }
            else
            {
                await _unitOfWork.ShoppingCart.UpdateAsync(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new InvalidOperationException("An error occurred while deleting or updating the cart item.", ex);
        }
    }

    public async Task<ShoppingCartDTO> GetCartItems(int businessEntityId)
    {

        try
        {
            var cartItems = await _unitOfWork.ShoppingCart.FindWithProjection(
                selector: Constants.GetShoppingCartSelectors(),
                predicate: cartItem => cartItem.BusinessEntityId == businessEntityId,
                includeProperties: ["Product", "Product.ProductProductPhotos.ProductPhoto"]);

            if (cartItems == null || !cartItems.Any())
            {
                return null;
            }

            return new ShoppingCartDTO()
            {
                Details = new Summary()
                {
                    TotalPrice = cartItems.Sum(item => item.TotalPrice),
                    ItemCount = cartItems.Sum(item => item.Quantity)
                },
                Items = cartItems
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while fetching the cart items.", ex);
        }
    }
}