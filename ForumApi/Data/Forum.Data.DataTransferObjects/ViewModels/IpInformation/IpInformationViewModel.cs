using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Forum.Data.DataTransferObjects.ViewModels.IpInformation
{
    public class IpInformationViewModel
    {
        [JsonProperty("ip")]
        public string IP { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        [JsonProperty("region_code")]
        public string RegionCode { get; set; }

        [JsonProperty("region_name")]
        public string RegionName { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }
}
