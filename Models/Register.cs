public class RegisterDTO
{
    public string PersonType { get; set; } = "IN";
    public bool NameStyle { get; set; } = false;
    public string? Title { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress1 { get; set; }
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
    public int EmailPromotion { get; set; } = 0;
    public int AddressTypeId { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string City { get; set; }
    public int StateProvinceId { get; set; } = 1;
    public string PostalCode { get; set; }
}
public class RegisterVM
{
    public string? Title { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress1 { get; set; }
    public string Password { get; set; }
    public int EmailPromotion { get; set; } = 0;
    public int AddressTypeId { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string City { get; set; }
    public int StateProvinceId { get; set; } = 1;
    public string PostalCode { get; set; }

}