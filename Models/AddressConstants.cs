public class AddressConstants
{
    public List<StateVM> States { get; set; }
    public List<AddressTypeVM> AddressTypes { get; set; }
}



public class AddressTypeDTO
{
    public int AddressTypeId { get; set; }
    public string Name { get; set; } = null!;
}

public class AddressTypeVM
{
    public int AddressTypeId { get; set; }
    public string Name { get; set; } = null!;
}

public class StateDTO
{
    public int StateProvinceId { get; set; }
    public string Name { get; set; } = null!;
}

public class StateVM
{
    public int StateProvinceId { get; set; }
    public string Name { get; set; } = null!;
}