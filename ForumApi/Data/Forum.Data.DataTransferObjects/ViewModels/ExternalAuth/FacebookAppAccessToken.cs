using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Forum.Data.DataTransferObjects.ViewModels.ExternalAuth
{
    public class FacebookAppAccessToken
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
