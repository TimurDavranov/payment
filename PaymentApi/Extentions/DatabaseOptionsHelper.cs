using Microsoft.Extensions.Options;

using Common.Models;

namespace Common.Helpers;

public class DatabaseOptionsHelper : IConfigureOptions<DatabaseOptionsModel>
{
    private const string DbOptionSectionName = "DatabaseOptions";
    private const string DbSchemeSectionName = "Database";
    private readonly IConfiguration _configuration;
    
    public DatabaseOptionsHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(DatabaseOptionsModel model)
    {
        var connectionString = _configuration.GetConnectionString(DbSchemeSectionName);
        
        model.ConnectionString = connectionString;
        
        _configuration.GetSection(DbOptionSectionName).Bind(model);
        
    }
}