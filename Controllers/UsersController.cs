


using System.Reflection.Metadata.Ecma335;
using FirebaseAdmin.Auth;
using Google.Api.Gax;
using Microsoft.AspNetCore.Mvc;

namespace web_dot_net_core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController
    {

        [HttpPost()]
        public async Task<ActionResult<string>> Post([FromBody] User user)
        {
            UserRecordArgs item = new UserRecordArgs();
            item.DisplayName = user.Name;
            item.Email = user.Email;
            item.Password = user.Password;
            item.EmailVerified = false;

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(item);

            // gen token

            string token = userRecord.Uid;

            string customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(token);
          
            Console.WriteLine($"User Uid: {token}");
            Console.WriteLine($"User Token: {customToken}");

            return "Successfully created new user";
        }

 
        [HttpPost(Name ="login")]
        public async Task<ActionResult<string>> PostLogin([FromBody] User user)
        {
             FirebaseToken  decodedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(user.Token);

             string uid = decodedToken.Uid;

            return "uid";
        }
      

        [HttpGet()]
        public async Task<ActionResult<string>> Get([FromBody] User user)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(user.Email);
            ExportedUserRecord userPassword = await getPasswordByUserAsync(userRecord);

            if (userPassword != null)
            {
                Console.WriteLine($"User Password: {userPassword.PasswordHash}");
            }
            

            return "";
        }

        private async Task<ExportedUserRecord?> getPasswordByUserAsync(UserRecord filterUser)
        {
            string uid = filterUser.Uid;

            var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
            var responses = pagedEnumerable.AsRawResponses().GetAsyncEnumerator();
            while (await responses.MoveNextAsync())
            {
                ExportedUserRecords response = responses.Current;
                foreach (ExportedUserRecord user in response.Users)
                {
                    Console.WriteLine($"User: {user.Uid}");
                    Console.WriteLine($"filterUser: {uid}");

                    if (user.Uid.Equals(uid))
                    {
                        return user;
                    }

                }
            }
            return null;
        }
    }

}
