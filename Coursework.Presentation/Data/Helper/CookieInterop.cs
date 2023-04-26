using Microsoft.JSInterop;
using System.Threading.Tasks;

public class CookieInterop
{
    private readonly IJSRuntime _jsRuntime;

    public CookieInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetCookieAsync(string name, string value, int days)
    {
        await _jsRuntime.InvokeVoidAsync("cookieInterop.setCookie", name, value, days);
    }
}
