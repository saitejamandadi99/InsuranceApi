using InsuranceApi.DTO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Helpers
{
    public static class AuthHelpers
    {
        public static async Task<string> Login(HttpClient client, string email, string password)
        {
            var request = new LoginRequestDTO
            {
                Email = email,
                Password = password
            };

            var response = await client.PostAsJsonAsync("/api/Auth/login", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

            return result!.JwtToken;
        }
    }
}
