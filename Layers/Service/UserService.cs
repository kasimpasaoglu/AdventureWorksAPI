using AdventureWorksAPI.Models.DMO;
using AutoMapper;

public interface IUserService
{
    Task RegisterUserAsync(RegisterDTO dto);
    List<StateDTO> GetAllStates();
    List<AddressTypeDTO> GetAddressTypes();
    Task<LoginResult> ValidateUserAsync(Login login);
    Task<bool> DeleteUserAsync(int businessEntityId);
}

public class UserService : IUserService
{
    public IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _configuration = configuration;
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
            var salt = EncryptHelper.GenerateSalt();
            var hashedPassword = EncryptHelper.HashPassword(dto.Password, salt);

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

    public List<StateDTO> GetAllStates()
    {

        return _unitOfWork.StateProvince.Find(
            predicate: x => true,
            selector: x => new StateDTO
            {
                StateProvinceId = x.StateProvinceId,
                Name = x.Name
            }
        ).Result.ToList();
    }

    public List<AddressTypeDTO> GetAddressTypes()
    {
        return _unitOfWork.AddressType.Find(
            predicate: x => true,
            selector: x => new AddressTypeDTO
            {
                AddressTypeId = x.AddressTypeId,
                Name = x.Name
            }
        ).Result.ToList();
    }

    public async Task<LoginResult> ValidateUserAsync(Login login)
    {
        #region 1 E-mail ile BusinessEntityId bul
        var emailRecord = await _unitOfWork.EmailAddress.FindSingle(
            predicate: x => x.EmailAddress1 == login.Email,
            selector: x => new { x.BusinessEntityId },
            includeProperties: null
        );
        if (emailRecord == null)
        {
            return new LoginResult
            {
                IsSuccessful = false,
                Message = "User not found with the provided email."
            };
        }

        var businessEntityId = emailRecord.BusinessEntityId;
        #endregion

        #region 2 Password tablosundan Salt ve Hashed Password'u al
        var passwordRecord = await _unitOfWork.Password.FindSingle(
            predicate: x => x.BusinessEntityId == businessEntityId,
            selector: x => new { x.PasswordHash, x.PasswordSalt },
            includeProperties: null
        );

        if (passwordRecord == null)
        {
            return new LoginResult
            {
                IsSuccessful = false,
                Message = "Password record not found for the user."
            };
        }
        #endregion

        #region 3 Kullanicinin girdigi sifreyi, salti kullanarak hasle
        var hashedInputPw = EncryptHelper.HashPassword(login.Password, passwordRecord.PasswordSalt);
        #endregion

        #region 4. Hash eslemesini kontrol et
        if (hashedInputPw != passwordRecord.PasswordHash)
        {
            return new LoginResult
            {
                IsSuccessful = false,
                Message = "Incorrect password."
            };
        }
        #endregion

        #region 5. Token olustur (giris basarili)
        var token = Token.Generator(_configuration, login.Email, businessEntityId);

        return new LoginResult
        {
            IsSuccessful = true,
            Message = "Login successful.",
            BusinessEntityId = businessEntityId,
            Token = token
        };
        #endregion
    }

    public async Task<bool> DeleteUserAsync(int businessEntityId)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        #region BusinessEntity Kontrolu
        var businessEntity = await _unitOfWork.BusinessEntity.FindSingle(
        x => x.BusinessEntityId == businessEntityId,
        selector: x => x
    );

        if (businessEntity == null)
        {
            throw new InvalidOperationException("User not found with the provided BusinessEntityId.");
        }
        #endregion

        try
        {
            #region 1- Email Adresi kaydini sil
            var email = await _unitOfWork.EmailAddress.FindSingle(
                x => x.BusinessEntityId == businessEntityId,
                selector: x => x
            );
            if (email != null)
            {
                await _unitOfWork.EmailAddress.RemoveAsync(email);
            }
            #endregion

            #region 2-Password kaydini sil
            var pasword = await _unitOfWork.Password.FindSingle(
                x => x.BusinessEntityId == businessEntityId,
                selector: x => x
            );

            if (pasword != null)
            {
                await _unitOfWork.Password.RemoveAsync(pasword);
            }
            #endregion

            #region 3-Address ve BusinessEntityAddress kayitlarini sil
            var businessEntityAddress = await _unitOfWork.BusinessEntityAddress.FindSingle(
                x => x.BusinessEntityId == businessEntityId,
                selector: x => x
            ); // businessentityaddress tablosundaki silinecek deger

            if (businessEntityAddress != null)
            {
                var address = await _unitOfWork.Address.FindSingle(
                    x => x.AddressId == businessEntityAddress.AddressId,
                    selector: x => x
                ); // Address tablosundaki silecenek deger

                // 1 once businessEntityAdress kalmasi gerekiyor
                await _unitOfWork.BusinessEntityAddress.RemoveAsync(businessEntityAddress);
                await _unitOfWork.SaveChangesAsync(); // iliskiyi kaldir

                // 2 sonra Address tablosundaki kaydin kalkmasi gerekiyor
                if (address != null)
                {
                    await _unitOfWork.Address.RemoveAsync(address);
                }
            }
            #endregion

            #region 4. Person kaydini sil
            var person = await _unitOfWork.Person.FindSingle(
                x => x.BusinessEntityId == businessEntityId,
                selector: x => x
            );

            if (person != null)
            {
                await _unitOfWork.Person.RemoveAsync(person);
            }
            #endregion

            #region  5.BusinessEntity kaydini sil
            await _unitOfWork.BusinessEntity.RemoveAsync(businessEntity);
            #endregion

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return false;
        }
    }
}