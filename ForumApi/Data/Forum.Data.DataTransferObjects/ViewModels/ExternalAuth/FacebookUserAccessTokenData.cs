using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Forum.Data.DataTransferObjects.ViewModels.ExternalAuth
{
    public class FacebookUserAccessTokenData
    {
        [JsonProperty("app_id")]
        public long AppId { get; set; }

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }
    }
}
