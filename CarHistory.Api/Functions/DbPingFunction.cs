using CarHistory.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace CarHistory.Api.Functions;

public class DbPingFunction
{
    private readonly AppDbContext _db;
    public DbPingFunction(AppDbContext db) => _db = db;

    [Function("DbPing")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dbping")] HttpRequestData req)
    {
        var canConnect = await _db.Database.CanConnectAsync();
        var count = canConnect ? await _db.CarEntries.CountAsync() : -1;

        return new OkObjectResult(new { canConnect, carEntries = count });
    }
}
