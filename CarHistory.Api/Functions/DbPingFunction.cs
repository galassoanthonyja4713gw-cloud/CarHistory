using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

public class DbPingFunction
{
    [Function("DbPing")]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "db-ping")] HttpRequestData req)
    {
        var res = req.CreateResponse(System.Net.HttpStatusCode.OK);
        res.WriteString("function alive");
        return res;
    }

    public class PingFunction
    {
        [Function("Ping")]
        public HttpResponseData Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequestData req)
        {
            var res = req.CreateResponse(HttpStatusCode.OK);
            res.Headers.Add("content-type", "text/plain; charset=utf-8");
            res.WriteString("pong-2026-01-08");
            return res;
        }
    }
}
