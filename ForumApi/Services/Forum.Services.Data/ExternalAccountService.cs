using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.Enums;
using Forum.Data.DataTransferObjects.ViewModels.ExternalAuth;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Forum.Services.Data.Utils;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using LoginInfo = Forum.Data.Models.Users.LoginInfo;

namespace Forum.Services.Data
{
    public class ExternalAccountService : AccountService, IExternalAccountService
    {
        private readonly FacebookSettings fbSettings;

        public ExternalAccountService(IOptions<FacebookSettings> fbSettings, UserManager<User> userManager, IHttpContextAccessor accessor, IOptions<GeoLocationSettings> geoLocationSettings, IOptions<JwtSettings> jwtSettings, ILogger<BaseService> logger, IMapper mapper, IRepository<User> userRepository, IRepository<LoginInfo> loginInfoRepository, SignInManager<User> signInManager) : base(userManager, accessor, geoLocationSettings, jwtSettings, logger, mapper, userRepository, loginInfoRepository, signInManager)
        {
            this.fbSettings = fbSettings.Value;
        }

        public async Task<string> FacebookLogin(FacebookLoginModel model)
        {
            var appAccessTokenResponse = await client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={fbSettings.AppId}&client_secret={fbSettings.AppSecret}&grant_type=client_credentials");

            var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);


            var userAccessTokenValidationResponse = await client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.Token}&access_token={appAccessToken.AccessToken}");
            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

            if (!userAccessTokenValidation.Data.IsValid)
            {
                throw new Exception("Invalid Facebook Token.");
            }

            var userInfoResponse = await client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={model.Token}");
            var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

            var user = await this.UserManager.FindByEmailAsync(userInfo.Email);
            if (user is null)
            {
                var appUser = new User
                {
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                };

                await this.UserManager.CreateAsync(appUser);
                await this.UserManager.AddToRoleAsync(appUser, Enum.GetName(typeof(Roles), 2));
            }

            user = await this.UserManager.FindByEmailAsync(userInfo.Email);

            return this.GenerateToken(user);
        }
    }
}
