public class RegisterDTO
{
    public string PersonType { get; set; } = "IN";
    public bool NameStyle { get; set; } = false;
    public string? Title { get; set; }
    required public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    required public string LastName { get; set; }
    required public string EmailAddress1 { get; set; }
    required public string Password { get; set; } // pasworld hashlenecek
    required public string PasswordSalt { get; set; } // salt kullanilacak
    public int EmailPromotion { get; set; }
    required public int AddressTypeId { get; set; }
    required public string AddressLine1 { get; set; }
    required public string? AddressLine2 { get; set; }
    required public string City { get; set; }
    required public int StateProvinceId { get; set; }
    required public string PostalCode { get; set; }
}
public class RegisterVM
{
    public string? Title { get; set; }
    required public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    required public string LastName { get; set; }
    required public string EmailAddress1 { get; set; }
    required public string Password { get; set; }
    public int EmailPromotion { get; set; }
    required public int AddressTypeId { get; set; }
    required public string AddressLine1 { get; set; }
    required public string? AddressLine2 { get; set; }
    required public string City { get; set; }
    required public int StateProvinceId { get; set; }
    required public string PostalCode { get; set; }

}