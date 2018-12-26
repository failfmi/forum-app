using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data.Models.Users;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Forum.WebApi.Middleware
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string[] AvailableMethods = { "POST", "PUT", "DELETE" };

        public RequestMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IServiceProvider provider)
        {
            var userManager = provider.GetService<UserManager<User>>();
            var returnMessage = new ReturnMessage { Message = "You are not authorized." };
            var serialized = JsonConvert.SerializeObject(returnMessage);

            if (AvailableMethods.Contains(context.Request.Method.ToUpper()))
            {
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var token = context.Request.Headers["Authorization"];
                    try
                    {
                        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(token.ToString().Split(" ")[1]);
                        var userName = decodedToken.Claims.First(c => c.Type == "unique_name").Value;
                        var user = userManager.Users.FirstOrDefault(u => u.UserName == userName);

                        if (user.IsActive == false)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync(serialized);
                            return;
                        }
                        await this.next.Invoke(context);
                    }
                    catch (Exception e)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync(serialized);
                        return;
                    }
                }
            }
            else
            {
                await this.next.Invoke(context);
            }
        }
    }
}
