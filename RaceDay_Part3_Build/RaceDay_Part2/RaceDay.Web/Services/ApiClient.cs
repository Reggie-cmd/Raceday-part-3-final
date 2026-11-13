using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RaceDay.Web.Services;

/// <summary>HTTP gateway. All application data is requested from the Part 2 API; MVC has no database access.</summary>
public sealed class ApiClient(IHttpClientFactory factory, IHttpContextAccessorAccessor accessor)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true };
    private HttpClient Client => factory.CreateClient("RaceDayApi");
    private Microsoft.AspNetCore.Http.ISession Session => accessor.HttpContext?.Session ?? throw new InvalidOperationException("No active HTTP session.");

    public async Task<(bool Success, string? Error, T? Data)> GetAsync<T>(string path) => await SendAsync<T>(HttpMethod.Get, path);
    public async Task<(bool Success, string? Error, T? Data)> PostAsync<T>(string path, object body) => await SendAsync<T>(HttpMethod.Post, path, body);
    public async Task<(bool Success, string? Error, T? Data)> PutAsync<T>(string path, object body) => await SendAsync<T>(HttpMethod.Put, path, body);
    public async Task<(bool Success, string? Error, T? Data)> DeleteAsync<T>(string path) => await SendAsync<T>(HttpMethod.Delete, path);
    public async Task<(bool Success, string? Error, T? Data)> PostFileAsync<T>(string path, Microsoft.AspNetCore.Http.IFormFile file)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        var cookie = Session.GetString("ApiCookie");
        if (!string.IsNullOrWhiteSpace(cookie)) request.Headers.Add("Cookie", cookie);
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", Path.GetFileName(file.FileName));
        request.Content = content;
        try { using var response = await Client.SendAsync(request); var body = await response.Content.ReadAsStringAsync(); if (!response.IsSuccessStatusCode) return (false, ExtractMessage(body) ?? $"API returned {(int)response.StatusCode}.", default); return (true, null, JsonSerializer.Deserialize<T>(body, Json)); }
        catch (HttpRequestException) { return (false, "Could not connect to the RaceDay API.", default); }
    }

    public async Task<(bool Success, string? Error, T? Data)> SendAsync<T>(HttpMethod method, string path, object? body = null, bool captureCookie = false)
    {
        using var request = new HttpRequestMessage(method, path);
        var cookie = Session.GetString("ApiCookie");
        if (!string.IsNullOrWhiteSpace(cookie)) request.Headers.Add("Cookie", cookie);
        if (body is not null) request.Content = new StringContent(JsonSerializer.Serialize(body, Json), Encoding.UTF8, "application/json");
        try
        {
            using var response = await Client.SendAsync(request);
            if (captureCookie && response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var authCookie = cookies.FirstOrDefault(x => x.StartsWith(".RaceDay.Auth=", StringComparison.Ordinal));
                if (authCookie is not null) Session.SetString("ApiCookie", authCookie.Split(';')[0]);
            }
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return (false, ExtractMessage(content) ?? $"API returned {(int)response.StatusCode} ({response.StatusCode}).", default);
            if (typeof(T) == typeof(string)) return (true, null, (T)(object)content);
            if (string.IsNullOrWhiteSpace(content)) return (true, null, default);
            return (true, null, JsonSerializer.Deserialize<T>(content, Json));
        }
        catch (HttpRequestException) { return (false, "Could not connect to the RaceDay API. Check that the API is running and that Api:BaseUrl is correct.", default); }
        catch (TaskCanceledException) { return (false, "The API request timed out. Please try again.", default); }
    }
    private static string? ExtractMessage(string json) { try { using var d = JsonDocument.Parse(json); if (d.RootElement.TryGetProperty("message", out var m)) return m.GetString(); if (d.RootElement.TryGetProperty("title", out var t)) return t.GetString(); } catch { } return null; }
}

// A tiny accessor wrapper keeps the session dependency explicit without storing session data in singleton services.
public interface IHttpContextAccessorAccessor { Microsoft.AspNetCore.Http.HttpContext? HttpContext { get; } }
public sealed class HttpContextAccessorAccessor(Microsoft.AspNetCore.Http.IHttpContextAccessor accessor) : IHttpContextAccessorAccessor { public Microsoft.AspNetCore.Http.HttpContext? HttpContext => accessor.HttpContext; }
