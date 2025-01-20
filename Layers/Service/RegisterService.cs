using AdventureWorksAPI.Models.DMO;
using AutoMapper;

public interface IRegisterService
{
    Task RegisterUserAsync(RegisterDTO dto);
}

public class RegisterService : IRegisterService
{
    private readonly IUnitOfWork _unitOfWork;
    public RegisterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

    }
    public async Task RegisterUserAsync(RegisterDTO dto)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            #region businessEntity
            var businessEntity = new BusinessEntity
            {
                Rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow
            };

            await _unitOfWork.BusinessEntity.AddAsync(businessEntity);
            await _unitOfWork.SaveChangesAsync(); // BusinessEntityId'yi almak icin save
            #endregion

            #region Person
            var person = new Person
            {
                BusinessEntityId = businessEntity.BusinessEntityId,
                PersonType = dto.PersonType,
                NameStyle = dto.NameStyle,
                Title = dto.Title,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                EmailPromotion = dto.EmailPromotion,
                ModifiedDate = DateTime.UtcNow,
                Rowguid = Guid.NewGuid()
            };
            await _unitOfWork.Person.AddAsync(person);
            #endregion


            #region EmailAddress
            var email = new EmailAddress
            {
                BusinessEntityId = businessEntity.BusinessEntityId,
                EmailAddress1 = dto.EmailAddress1,
                Rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow
            };

            await _unitOfWork.EmailAddress.AddAsync(email);
            await _unitOfWork.SaveChangesAsync(); // EmailAddressId almak icin (lazim olursa diye)
            #endregion

            #region Password
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(dto.Password, salt);

            var password = new Password
            {
                BusinessEntityId = businessEntity.BusinessEntityId,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow
            };

            await _unitOfWork.Password.AddAsync(password);
            #endregion

            #region Address
            var address = new Address
            {
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                StateProvinceId = dto.StateProvinceId,
                PostalCode = dto.PostalCode,
                Rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
            };

            await _unitOfWork.Address.AddAsync(address);
            await _unitOfWork.SaveChangesAsync(); // AddressId'yi almak icin save
            #endregion

            #region BusinessEntityAddress
            var bussinesEntityAddress = new BusinessEntityAddress
            {
                BusinessEntityId = businessEntity.BusinessEntityId,
                AddressId = address.AddressId,
                AddressTypeId = dto.AddressTypeId,
                Rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
            };

            await _unitOfWork.BusinessEntityAddress.AddAsync(bussinesEntityAddress);

            await _unitOfWork.SaveChangesAsync();
            #endregion

            await _unitOfWork.CommitTransactionAsync(); // degisiklikleri commitle

        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(); // degisiklikleri geri al!!!
            throw new InvalidOperationException("An error occurred while registering the user. See inner exception for details.", ex);
        }

    }




    private string GenerateSalt()
    {
        var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        var saltBytes = new byte[8];
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes).Substring(0, 10);
    }

    private string HashPassword(string password, string salt)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var combinedPassword = password + salt;
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combinedPassword));
        return Convert.ToBase64String(hashBytes);
    }
}