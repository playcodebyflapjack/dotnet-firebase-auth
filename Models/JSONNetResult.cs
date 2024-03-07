using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace web_dot_net_core.Controllers
{
    public class JSONNetResult: ActionResult
    {
        private readonly JObject _data;
        public JSONNetResult(JObject data)
        {
            _data = data;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.WriteAsync(_data.ToString(Newtonsoft.Json.Formatting.None));
        }
        
    }
}