using AdventureWorksAPI.Models.DMO;

public interface ICartService
{
    Task<bool> InsertNewItemAsync(InsertVM item, int businessEntityId);
}

public class CartService : ICartService
{
    private IUnitOfWork _unitOfWork;
    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> InsertNewItemAsync(InsertVM item, int businessEntityId)
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

}