namespace Common.Models;

public class DatabaseOptionsModel
{
    public string ConnectionString {get;set;}
    public int MaxRetryCount {get;set;}
    public int CommandTimeOut {get;set;}
    public bool EnableDetaildeErrors {get;set;}
    public bool EnableSensitiveDataLogging {get;set;}
}