﻿using System;
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
        private const string GmailLoginVerifier = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}";
        private const string UserBannedErrorMessage = "You are banned! Contact admin for further information.";

        public ExternalAccountService(IOptions<FacebookSettings> fbSettings, UserManager<User> userManager, IHttpContextAccessor accessor, IOptions<GeoLocationSettings> geoLocationSettings, IOptions<JwtSettings> jwtSettings, ILogger<BaseService> logger, IMapper mapper, IRepository<User> userRepository, IRepository<LoginInfo> loginInfoRepository, SignInManager<User> signInManager) : base(userManager, accessor, geoLocationSettings, jwtSettings, logger, mapper, userRepository, loginInfoRepository, signInManager)
        {
            this.fbSettings = fbSettings.Value;
        }

        public async Task<string> FacebookLogin(ExternalLoginModel model)
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
                    DateRegistered = DateTime.UtcNow
                };

                await this.UserManager.CreateAsync(appUser);
                await this.UserManager.AddToRoleAsync(appUser, Enum.GetName(typeof(Roles), 2));
            }

            user = await this.UserManager.FindByEmailAsync(userInfo.Email);

            if (user.IsActive == false)
            {
                throw new UnauthorizedAccessException(UserBannedErrorMessage);
            }

            await this.LoginInfo(user.Id);

            return this.GenerateToken(user);
        }

        public async Task<string> GmailLogin(ExternalLoginModel model)
        {
            var accessTokenValidationResponse = await this.client.GetAsync(string.Format(GmailLoginVerifier, model.Token));

            try
            {
                accessTokenValidationResponse.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new Exception("Invalid Google+ Token.");
            }

            var content = JsonConvert.DeserializeObject<GoogleApiTokenInfo>(await accessTokenValidationResponse.Content.ReadAsStringAsync());

            var user = await this.UserManager.FindByEmailAsync(content.email);

            if (user is null)
            {
                var appUser = new User
                {
                    Email = content.email,
                    UserName = content.email,
                    DateRegistered = DateTime.UtcNow
                };

                await this.UserManager.CreateAsync(appUser);
                await this.UserManager.AddToRoleAsync(appUser, Enum.GetName(typeof(Roles), 2));
            }
            else
            {
                if (user.UserName != content.email)
                {
                    throw new Exception("Email is already taken.");
                }
            }

            user = await this.UserManager.FindByEmailAsync(content.email);

            if (user.IsActive == false)
            {
                throw new UnauthorizedAccessException(UserBannedErrorMessage);
            }

            await this.LoginInfo(user.Id);

            return this.GenerateToken(user);
        }
    }
}
