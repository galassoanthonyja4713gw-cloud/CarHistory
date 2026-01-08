using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;

public class ConfigCheckFunction
{
    private readonly IConfiguration _config;

    public ConfigCheckFunction(IConfiguration config)
    {
        _config = config;
    }

    [Function("ConfigCheck")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "config-check")] HttpRequestData req)
    {
        var fromGetConnectionString = _config.GetConnectionString("Default");
        var fromSection = _config["ConnectionStrings:Default"];
        var fromEnvDouble = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        var fromEnvSingle = Environment.GetEnvironmentVariable("ConnectionStrings_Default");

        var res = req.CreateResponse(HttpStatusCode.OK);

        await res.WriteStringAsync(
            "{" +
            $"\"hasGetConnectionString\":{Bool(fromGetConnectionString)}," +
            $"\"hasConnectionStringsSection\":{Bool(fromSection)}," +
            $"\"hasEnvDoubleUnderscore\":{Bool(fromEnvDouble)}," +
            $"\"hasEnvSingleUnderscore\":{Bool(fromEnvSingle)}," +
            $"\"previewGetConnectionString\":\"{EscapeJson(Redact(fromGetConnectionString))}\"," +
            $"\"previewEnvDoubleUnderscore\":\"{EscapeJson(Redact(fromEnvDouble))}\"" +
            "}"
        );

        return res;
    }

    private static string Redact(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        var preview = s;

        var pwdIdx = preview.IndexOf("Password=", StringComparison.OrdinalIgnoreCase);
        if (pwdIdx >= 0)
        {
            var end = preview.IndexOf(';', pwdIdx);
            if (end < 0) end = preview.Length;
            preview = preview.Remove(pwdIdx, end - pwdIdx).Insert(pwdIdx, "Password=***");
        }

        return preview;
    }

    private static string Bool(string? s) => (!string.IsNullOrWhiteSpace(s)).ToString().ToLowerInvariant();

    private static string EscapeJson(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "");
}
