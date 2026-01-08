using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

public class DbPingFunction
{
    [Function("Ping")]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("pong");
        return res;
    }
}
