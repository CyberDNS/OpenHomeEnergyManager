using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;


namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication
{
    public class LoginClient
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        private static Random _random = new Random();

        public LoginClient(ILogger<LoginClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<TokenMaterial> Login(string email, string password)
        {
            string codeVerifier = RandomString(86);
            string codeChallenge = Sha256HexHashString(codeVerifier);

            var defaultParameters = GetDefaultParameters(codeChallenge, email);

            var step1Response = await LoginStep1(email, password, defaultParameters);
            var code = await LoginStep2(step1Response, defaultParameters);
            var step3Response = await LoginStep3(code, codeVerifier);
            var step4Response = await LoginStep4(step3Response);

            TokenMaterial tokens = new TokenMaterial()
            {
                AccessToken = step4Response.AccessToken,
                RefreshToken = step3Response.RefreshToken
            };

            return tokens;
        }


        private async Task<Step1ResponseDto> LoginStep1(string email, string password, Dictionary<string, string> defaultParameters)
        {
            string query = QueryHelpers.AddQueryString("https://auth.tesla.com/oauth2/v3/authorize", defaultParameters);

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();

            IEnumerable<string> cookie = response.Headers.GetValues("Set-Cookie").Select(c => c.Split(";").First());
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);

            string content = await response.Content.ReadAsStringAsync();
            var step2Dto = new Step1ResponseDto(content, email, password);
            return step2Dto;
        }

        private async Task<string> LoginStep2(Step1ResponseDto step2Dto, Dictionary<string, string> defaultParameters)
        {
            string query = QueryHelpers.AddQueryString("https://auth.tesla.com/oauth2/v3/authorize", defaultParameters);

            FormUrlEncodedContent form = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "_csrf", step2Dto.Csrf },
                { "_phase", step2Dto.Phase },
                { "_process", step2Dto.Process },
                { "transaction_id", step2Dto.TransactionId },
                { "cancel", step2Dto.Cancel },
                { "identity", step2Dto.Email },
                { "credential", step2Dto.Password }
            });

            var response = await _httpClient.PostAsync(query, form);

            if (response.StatusCode != System.Net.HttpStatusCode.Redirect)
            {
                response.EnsureSuccessStatusCode();
            }

            var match = Regex.Match(response.Headers.Location.Query, @"[?&]code=(?'code'\w+)");

            if (match.Success)
            {
                return match.Groups["code"].ToString();
            }

            throw new ApplicationException("Tesla login step 2 failed");
        }

        private async Task<Step3ResponseDto> LoginStep3(string code, string codeVerifier)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "client_id", "ownerapi" },
                { "code", code },
                { "code_verifier", codeVerifier },
                { "redirect_uri", "https://auth.tesla.com/void/callback" }
            };

            JsonContent json = JsonContent.Create(parameters);
            string query = "https://auth.tesla.com/oauth2/v3/token";

            var response = await _httpClient.PostAsync(query, json);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<Step3ResponseDto>();
            return content;
        }

        private async Task<Step4ResponseDto> LoginStep4(Step3ResponseDto step3Response)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", step3Response.AccessToken);

            var parameters = new Dictionary<string, string>()
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                { "client_id", "81527cff06843c8634fdc09e8ac0abefb46ac849f38fe1e431c2ef2106796384" },
                { "client_secret", "c7257eb71a564034f9419ee651c7d0e5f7aa6bfbd18bafb5c5c033b093bb2fa3" }
            };

            JsonContent json = JsonContent.Create(parameters);
            string query = "https://owner-api.teslamotors.com/oauth/token";

            var response = await _httpClient.PostAsync(query, json);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<Step4ResponseDto>();
            return content;
        }

        private static Dictionary<string, string> GetDefaultParameters(string codeChallenge, string email)
        {
            return new Dictionary<string, string>()
            {
                { "client_id", "ownerapi" },
                { "code_challenge", codeChallenge },
                { "code_challenge_method", "S256" },
                { "redirect_uri", "https://auth.tesla.com/void/callback" },
                { "response_type", "code" },
                { "scope", "openid email offline_access" },
                { "state", "OpenHomeEnergyManager" },
                { "login_hint", email }
            };
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private static string Sha256HexHashString(string input)
        {
            string hashString;
            using (var sha256 = SHA256Managed.Create())
            {
                var hash = sha256.ComputeHash(Encoding.Default.GetBytes(input));
                hashString = ToHex(hash, false);
            }

            return hashString;
        }

        private static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }
    }
}
