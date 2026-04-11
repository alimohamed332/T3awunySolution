using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Application.JwtFeatures
{
    public class RefreshTokenHandler
    {
        public RefreshToken GenerateRefreshToken()
        {
            //var randomNumber = new byte[32];
            //var generator = new RNGCryptoServiceProvider();
            //generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow

            };
        }
    }
}