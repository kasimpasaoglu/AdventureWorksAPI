
public class FilterAttribute : Attribute
{
    public string Value { get; set; }
    public FilterAttribute(string value)
    {
        this.Value = value;
    }
}