
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Auth.Hash;
using FirebaseAdmin.Auth.Providers;
using Google.Apis.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace web_dot_net_core.Controllers
{
    public class ServicesFirebase
    {
        public readonly string keyServiceApi;
        public ServicesFirebase(string keyServiceApi)
        {
            this.keyServiceApi = keyServiceApi;
        }

        public async Task<ExportedUserRecord?> getPasswordByUserAsync(UserRecord filterUser)
        {
            string uid = filterUser.Uid;

            var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
            var responses = pagedEnumerable.AsRawResponses().GetAsyncEnumerator();
            while (await responses.MoveNextAsync())
            {
                ExportedUserRecords response = responses.Current;
                foreach (ExportedUserRecord user in response.Users)
                {

                    if (user.Uid.Equals(uid))
                    {
                        return user;
                    }

                }
            }
            return null;
        }

        public async Task<UserRecord?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (FirebaseAuthException)
            {
                return null;
            }
        }


        public async Task<UserRecord> CreateUserAsync(User user)
        {
            UserRecordArgs item = new UserRecordArgs
            {
                DisplayName = user.Name,
                Email = user.Email,
                Password = user.Password,
                EmailVerified = false
            };

            return await FirebaseAuth.DefaultInstance.CreateUserAsync(item);
        }


        public async Task DeleteUserAsync(string uid)
        {
             await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
        }

        public async Task<JObject> Login(
            User user)
        {

            JObject objBody = new JObject
            {
                { "email", user.Email },
                { "password", user.Password },
                { "return_secure_token", true }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,

                RequestUri = new Uri("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + this.keyServiceApi),

                Content = new StringContent(objBody.ToString()),

            };

            try
            {
                using var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("request not success");
                }

                JObject responseJson = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());

                return responseJson;

            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace);
                throw;
            }
        }

        public async Task<UserRecord> UpdateUser(string uid,User user)
        {
            UserRecordArgs args = new UserRecordArgs()
            {
                Uid = uid,
                Email = user.Email,
                Password = user.Password,
                DisplayName = user.Name
            };

           return await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
        }
    }
}
