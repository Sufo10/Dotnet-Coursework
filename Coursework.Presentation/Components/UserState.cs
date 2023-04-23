using Microsoft.JSInterop;
using System.Text.Json;

public class UserState
{
    public bool IsLoggedIn { get; private set; }
    public string Username { get; private set; }
    public string Role { get; private set; }

    public event Action OnChange;

    private readonly IJSRuntime _jsRuntime;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserState(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        LoadStateFromStorage();
    }

    public void SetUser(string username, string role)
    {
        Username = username;
        Role = role;
        IsLoggedIn = true;
        NotifyStateChanged();
        SaveStateToStorage();
    }

    public void ClearUser()
    {
        Username = null;
        Role = null;
        IsLoggedIn = false;
        NotifyStateChanged();
        SaveStateToStorage();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    private async void LoadStateFromStorage()
    {
        var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userState");
        if (!string.IsNullOrEmpty(userJson))
        {
            var userState = JsonSerializer.Deserialize<UserState>(userJson, _jsonOptions);
            IsLoggedIn = userState.IsLoggedIn;
            Username = userState.Username;
            Role = userState.Role;
            NotifyStateChanged();
        }
    }

    private async void SaveStateToStorage()
    {
        var userJson = JsonSerializer.Serialize(this, _jsonOptions);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userState", userJson);
    }
}