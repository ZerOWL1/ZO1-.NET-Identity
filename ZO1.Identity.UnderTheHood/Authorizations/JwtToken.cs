using System;
using Newtonsoft.Json;

namespace ZO1.Identity.UnderTheHood.Authorizations
{
    public class JwtToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}