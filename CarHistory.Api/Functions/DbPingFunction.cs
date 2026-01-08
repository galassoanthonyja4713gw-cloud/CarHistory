using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

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
}
