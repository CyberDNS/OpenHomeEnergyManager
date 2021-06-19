using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication
{
    class Step1ResponseDto
    {
        public string Csrf { get; }
        public string Phase { get; }
        public string Process { get; }
        public string TransactionId { get; }
        public string Cancel { get; }
        public string Email { get; }
        public string Password { get; }

        public Step1ResponseDto(string responseStep1, string email, string password)
        {
            Csrf = ExtractValue(responseStep1, "_csrf");
            Phase = ExtractValue(responseStep1, "_phase");
            Process = ExtractValue(responseStep1, "_process");
            TransactionId = ExtractValue(responseStep1, "transaction_id");
            Cancel = ExtractValue(responseStep1, "cancel");
            Email = email;
            Password = password;
        }

        private static string ExtractValue(string html, string name)
        {
            var match = Regex.Match(html, $"name=\"{name}\".+value=\"(?'value'[^\"]*)\"");

            if (match.Success)
            {
                return match.Groups["value"].Value.ToString();
            }

            return null;
        }
    }
}
