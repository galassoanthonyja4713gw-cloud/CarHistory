using CarHistory.Api.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CarHistory.Api.Functions
{
    public class DbPingFunction
    {
        private readonly AppDbContext _db;

        public DbPingFunction(AppDbContext db)
        {
            _db = db;
        }

        [Function("DbPing")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "db-ping")] HttpRequestData req)
        {
            try
            {
                await _db.Database.OpenConnectionAsync();
                await _db.Database.CloseConnectionAsync();

                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteStringAsync("db ok");
                return ok;
            }
            catch (Exception ex)
            {
                var fail = req.CreateResponse(HttpStatusCode.InternalServerError);
                await fail.WriteStringAsync(ex.ToString());
                return fail;
            }
        }
    }
}
