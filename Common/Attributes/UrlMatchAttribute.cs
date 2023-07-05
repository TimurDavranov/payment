namespace Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class UrlMatchAttribute : Attribute
{
    private string value;
    
    public UrlMatchAttribute(string value)
    {
        this.value = value;
    }
    
    public string Value
    {
        get {return value;}
    }
}