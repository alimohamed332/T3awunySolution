using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;
using T3awuny.Core.Entities;

namespace T3awuny.Infrastructure.Services
{
    public class AIRequestService : IAIRequestService
    {
        private readonly IConfiguration _configuration;
        private readonly string SecretKey;
        public AIRequestService(IConfiguration configuration)
        {
            _configuration = configuration;
            SecretKey = _configuration["AI:SecretKey"]??"";
        }

       
        private const string Algorithm = SecurityAlgorithms.HmacSha256;

        private string CreateAccessToken(string userId, string role = "trader", int expiresMinutes = 60)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, Algorithm);

            var claims = new[]
            {
            new Claim("user_id", userId),
            new Claim("role", role)
        };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> SenChatBot(string message)
        {
            string token = CreateAccessToken(userId: "1234", role:"trader");
            Console.WriteLine("Generated token: " + token);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new { message = message };

            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://osama-2004-agri-ai-egypt.hf.space/api/v1/chat",
                jsonContent
            );

            string responseText = await response.Content.ReadAsStringAsync();

            var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseText);

            //Console.WriteLine(chatResponse?.Reply);

            //Console.WriteLine($"Status: {response.StatusCode}");
            //Console.WriteLine($"Response: {responseText}");

            return chatResponse?.Reply ?? string.Empty;
        }
    }
}
