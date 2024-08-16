using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Almond;

public sealed class ConfigurationDataStore(IConfiguration configuration) : IDataStore
{
    public Task ClearAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync<T>(string key)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetAsync<T>(string key)
    {
        var tokenData = configuration[Configuration.EmailToken];

        var tcs = new TaskCompletionSource<T>();

        if (string.IsNullOrWhiteSpace(tokenData))
        {
            tcs.SetResult(default);
        }
        else
        {
            var response = JsonSerializer.Deserialize<TokenResponse>(tokenData);

            if (response is null)
                tcs.SetResult(default);
            else
                tcs.SetResult((T)(object)response);
        }

        return tcs.Task;
    }

    public Task StoreAsync<T>(string key, T value)
    {
        string tokenData = JsonSerializer.Serialize(value);

        Console.WriteLine("Save Token data for future use");
        Console.WriteLine(tokenData);
        Console.WriteLine();

        return Task.CompletedTask;
    }
}
