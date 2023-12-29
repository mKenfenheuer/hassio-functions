using System.Text.Json;
using HAFunctions.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HAFunctions.Shared;

public class DataStore : DbContext
{
    public DataStore(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    private DbSet<DataObject> Objects { get; set; }

    public async Task Set<T>(string key, T data)
    {
        var obj = await Objects.FirstOrDefaultAsync(o => o.Key == key);
        if (obj == null)
        {
            obj = new DataObject()
            {
                Key = key,
                JsonData = JsonSerializer.Serialize<T>(data)
            };
            await Objects.AddAsync(obj);
        }
        else
        {
            obj.JsonData = JsonSerializer.Serialize<T>(data);
            Objects.Update(obj);
        }
        await SaveChangesAsync();
    }
    public async Task<T?> Get<T>(string key)
    {
        var obj = await Objects.FirstOrDefaultAsync(o => o.Key == key);
        if (obj == null || obj.JsonData == null)
        {
           return default;
        }
        else
        {
            return JsonSerializer.Deserialize<T>(obj.JsonData);
        }
    }
}