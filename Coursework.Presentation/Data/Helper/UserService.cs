using System.Text.Json;
using Blazored.LocalStorage;
using Coursework.Presentation.Data.Models;

public class UserService
{
    private readonly ISyncLocalStorageService _localStorage;

    public UserService(ISyncLocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public void SetUser(UserInfo user)
    {
        var jsonString = JsonSerializer.Serialize(user);
        _localStorage.SetItem("user", jsonString);
    }

    public UserInfo GetUser()
    {
        var jsonString = _localStorage.GetItem<string>("user");

        if (string.IsNullOrEmpty(jsonString))
        {
            return null;
        }

        return JsonSerializer.Deserialize<UserInfo>(jsonString);
    }
}
