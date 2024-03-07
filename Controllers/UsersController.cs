using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Auth.Hash;
using Google.Api.Gax;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace web_dot_net_core.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class UsersController  : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ServicesFirebase servicesFirebase;
        private readonly string keyServiceApi = "";
        public UsersController(ILogger<UsersController> logger )
        {
            this.servicesFirebase = new ServicesFirebase(keyServiceApi);
            this._logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] User user)
        {
            UserRecord userRecord = await servicesFirebase.CreateUserAsync(user);
        
            return JsonSerializer.Serialize(userRecord);
        }

         [HttpGet]
        public async Task<ActionResult<string>> Get([FromBody] User user)
        {
            try
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(user.Email);
              
                return new JsonResult(userRecord);
            }
            catch(Exception error)
            {
                return error.Message;
            }
        }

        [HttpPut("{uid}")]
        public async Task<ActionResult<string>> Update(string uid,[FromBody] User user)
        {
            UserRecord userRecord = await servicesFirebase.UpdateUser(uid,user);

            return new JsonResult(userRecord);
        }


        [HttpDelete("{uid}")]
        public async Task<ActionResult<string>> Delete(string uid)
        {
            await servicesFirebase.DeleteUserAsync(uid);

            return "Successfully deleted user.";
        }


        
        [HttpPost("/api/login")]
        public async Task<ActionResult<string>> PostLogin([FromBody] User user)
        {
            try
            {
                JObject responseJson = await servicesFirebase.Login(user);
                return new JSONNetResult(responseJson);
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return error.Message;
            }
        }

    }

}
