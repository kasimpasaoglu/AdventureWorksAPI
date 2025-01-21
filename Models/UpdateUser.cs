public class UpdateUserDTO
{
    public int BusinessEntityId { get; set; }
    public string? Title { get; set; } = null;
    public string? FirstName { get; set; } = null;
    public string? MiddleName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public string? EmailAddress1 { get; set; } = null;
    public string? Password { get; set; } = null;
    public string? PasswordSalt { get; set; } = null;
    public int? EmailPromotion { get; set; } = null;
    public int? AddressTypeId { get; set; } = null;
    public string? AddressLine1 { get; set; } = null;
    public string? AddressLine2 { get; set; } = null;
    public string? City { get; set; } = null;
    public int? StateProvinceId { get; set; } = null;
    public string? PostalCode { get; set; } = null;
}

public class UpdateUserVM
{
    public int BusinessEntityId { get; set; }
    public string? Title { get; set; } = null;
    public string? FirstName { get; set; } = null;
    public string? MiddleName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public string? EmailAddress1 { get; set; } = null;
    public string? Password { get; set; } = null;
    public string? PasswordSalt { get; set; } = null;
    public int? EmailPromotion { get; set; } = null;
    public int? AddressTypeId { get; set; } = null;
    public string? AddressLine1 { get; set; } = null;
    public string? AddressLine2 { get; set; } = null;
    public string? City { get; set; } = null;
    public int? StateProvinceId { get; set; } = null;
    public string? PostalCode { get; set; } = null;
}