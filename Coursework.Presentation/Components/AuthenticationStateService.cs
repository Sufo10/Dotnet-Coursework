using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Coursework.Presentation.Components
{
    public class AuthenticationStateService
    {
        public event Action OnUserStateChanged;
        private readonly IJSRuntime _jsRuntime;

        public AuthenticationStateService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetLoggedInUserAsync(string username, string role, bool? isVerified = null)
        {
            var userObject = new LoggedInUser { Username = username, Role = role, IsVerified  = isVerified};
            string userJson = JsonSerializer.Serialize(userObject);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "loggedInUser", userJson);
            OnUserStateChanged?.Invoke();
        }

        public async Task<(string Username, string Role)> GetLoggedInUserAsync()
        {
            string userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "loggedInUser");

            if (string.IsNullOrEmpty(userJson))
            {
                return (null, null);
            }

            var userObject = JsonSerializer.Deserialize<LoggedInUser>(userJson);
            return (userObject.Username, userObject.Role);
        }

        public async Task RemoveLoggedInUserAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loggedInUser");
            OnUserStateChanged?.Invoke();
        }
    }

    public class LoggedInUser
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public bool? IsVerified { get; set;}
    }
}
