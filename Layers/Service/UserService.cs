using AdventureWorksAPI.Models.DMO;

public interface IUserService
{
    Task RegisterUserAsync(RegisterDTO dto);
    List<StateDTO> GetAllStates();
    List<AddressTypeDTO> GetAddressTypes();
    Task<LoginResult> ValidateUserAsync(Login login);
    Task<bool> DeleteUserAsync(int businessEntityId);
    Task<bool> UpdateUserAsync(UpdateUserDTO dto);

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
            var email = await _unitOfWork.EmailAddress.FindSingle<EmailAddress>(x => x.BusinessEntityId == businessEntityId);
            if (email != null)
            {
                await _unitOfWork.EmailAddress.RemoveAsync(email);
            }
            #endregion

            #region 2-Password kaydini sil
            var pasword = await _unitOfWork.Password.FindSingle<Password>(x => x.BusinessEntityId == businessEntityId);

            if (pasword != null)
            {
                await _unitOfWork.Password.RemoveAsync(pasword);
            }
            #endregion

            #region 3-Address ve BusinessEntityAddress kayitlarini sil
            var businessEntityAddress = await _unitOfWork.BusinessEntityAddress.FindSingle<BusinessEntityAddress>(x => x.BusinessEntityId == businessEntityId);
            // businessentityaddress tablosundaki silinecek deger

            if (businessEntityAddress != null)
            {
                var address = await _unitOfWork.Address.FindSingle<Address>(x => x.AddressId == businessEntityAddress.AddressId);
                // Address tablosundaki silecenek deger

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
            var person = await _unitOfWork.Person.FindSingle<Person>(x => x.BusinessEntityId == businessEntityId);

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

    public async Task<bool> UpdateUserAsync(UpdateUserDTO dto)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            #region  1. BusinessEntity doğrulaması
            var businessEntity = await _unitOfWork.BusinessEntity.FindSingle<BusinessEntity>(
                x => x.BusinessEntityId == dto.BusinessEntityId
            );

            if (businessEntity == null)
            {
                throw new InvalidOperationException("User not found with the provided BusinessEntityId.");
            }
            #endregion

            #region  2. Person güncellemesi
            var person = await _unitOfWork.Person.FindSingle<Person>(
                x => x.BusinessEntityId == dto.BusinessEntityId
            );

            if (person != null)
            {
                if (dto.Title != null) person.Title = dto.Title;
                if (dto.FirstName != null) person.FirstName = dto.FirstName;
                if (dto.MiddleName != null) person.MiddleName = dto.MiddleName;
                if (dto.LastName != null) person.LastName = dto.LastName;
                if (dto.EmailPromotion.HasValue) person.EmailPromotion = dto.EmailPromotion.Value;

                await _unitOfWork.Person.UpdateAsync(person);
            }
            #endregion

            #region 3. EmailAddress güncellemesi
            var email = await _unitOfWork.EmailAddress.FindSingle<EmailAddress>(
                x => x.BusinessEntityId == dto.BusinessEntityId
            );

            if (email != null && dto.EmailAddress1 != null)
            {
                email.EmailAddress1 = dto.EmailAddress1;
                await _unitOfWork.EmailAddress.UpdateAsync(email);
            }
            #endregion

            #region 4. Password güncellemesi
            if (!string.IsNullOrEmpty(dto.Password))
            {
                var password = await _unitOfWork.Password.FindSingle<Password>(
                    x => x.BusinessEntityId == dto.BusinessEntityId
                );

                if (password != null)
                {
                    var salt = EncryptHelper.GenerateSalt();
                    var hashedPassword = EncryptHelper.HashPassword(dto.Password, salt);

                    password.PasswordSalt = salt;
                    password.PasswordHash = hashedPassword;
                    await _unitOfWork.Password.UpdateAsync(password);
                }
            }
            #endregion

            #region 5. Address güncellemesi
            var businessEntityAddress = await _unitOfWork.BusinessEntityAddress.FindSingle<BusinessEntityAddress>(
                x => x.BusinessEntityId == dto.BusinessEntityId
            );

            if (businessEntityAddress != null)
            {
                var address = await _unitOfWork.Address.FindSingle<Address>(
                    x => x.AddressId == businessEntityAddress.AddressId
                );

                if (address != null)
                {
                    if (dto.AddressLine1 != null) address.AddressLine1 = dto.AddressLine1;
                    if (dto.AddressLine2 != null) address.AddressLine2 = dto.AddressLine2;
                    if (dto.City != null) address.City = dto.City;
                    if (dto.StateProvinceId.HasValue) address.StateProvinceId = dto.StateProvinceId.Value;
                    if (dto.PostalCode != null) address.PostalCode = dto.PostalCode;

                    await _unitOfWork.Address.UpdateAsync(address);
                }

                if (dto.AddressTypeId.HasValue)
                {
                    businessEntityAddress.AddressTypeId = dto.AddressTypeId.Value;
                    await _unitOfWork.BusinessEntityAddress.UpdateAsync(businessEntityAddress);
                }
            }
            #endregion

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