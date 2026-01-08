using CarHistory.Api.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

public class DbPingFunction
{
    private readonly AppDbContext _db;

    public DbPingFunction(AppDbContext db) => _db = db;

    [Function("db-ping")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "db-ping")] HttpRequestData req)
    {
        try
        {
            // simplest possible DB touch
            await _db.Database.OpenConnectionAsync();
            await _db.Database.CloseConnectionAsync();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            ok.WriteString("db ok");
            return ok;
        }
        catch (Exception ex)
        {
            var fail = req.CreateResponse(HttpStatusCode.InternalServerError);
            fail.WriteString(ex.ToString()); // TEMP: remove after debugging
            return fail;
        }
    }
}
