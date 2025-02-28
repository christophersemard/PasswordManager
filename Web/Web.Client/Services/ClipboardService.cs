using Microsoft.JSInterop;
using System.Threading.Tasks;

public interface IClipboardService
{
    Task CopyToClipboardAsync(string text);
}

public class ClipboardService : IClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task CopyToClipboardAsync(string text)
    {
        await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}