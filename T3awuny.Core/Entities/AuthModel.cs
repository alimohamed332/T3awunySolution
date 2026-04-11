using System.Text.Json.Serialization;

namespace T3awuny.Core.Entities
{
        public class AuthModel
        {
            public string Message { get; set; } = string.Empty;
            public bool IsAuthenticated { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public List<string> Roles { get; set; } = new List<string>();
            public string Token { get; set; } = string.Empty;
            //public DateTime ExpiresOn { get; set; } we will return the refresh token expiration date instead of the access token expiration date, because the access token will be renewed when the refresh token is still valid, so the client can use the same access token until the refresh token expires
            [JsonIgnore] // we will not return the refresh token to the client, because the client will use the same access token until the refresh token expires, so there is no need to return the refresh token to the client
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime RefreshTokenExpiration { get; set; }
        }
}

