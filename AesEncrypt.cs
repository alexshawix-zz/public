using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Answers
{
    class Program
    {
        static void Main(string[] args)
        {
            string ssoCallbackUrl = "<answers sso callback url>";
            string keyId = "<your api key id>";
            string secret = "<your api key secret>";

            UserData userData = new UserData
            {
                Id = "<your internal id>",
                Email = "<user email>",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            string jsonUserData = JsonConvert.SerializeObject(userData);

            string token = GenerateEncryptedToken(jsonUserData, secret);

            string redirectUrl = $"{ssoCallbackUrl}&token={token}&key={keyId}";

            Console.WriteLine(redirectUrl);
        }

        public static string GenerateEncryptedToken(string json, string secret)
        {

            using (var aes = new AesManaged())
            {
                HashAlgorithm algorithm = SHA1.Create();
                aes.Key = algorithm.ComputeHash(Encoding.UTF8.GetBytes(secret)).Take(16).ToArray();
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        cs.Write(data, 0, data.Length);
                    }

                    byte[] encrypted = ms.ToArray();
                    return Convert.ToBase64String(encrypted).Replace('+', '-').Replace('/', '_').Replace("=", "");
                }
            }
        }

        public class UserData
        {
            [JsonProperty("id", Required = Required.Always)]
            public string Id { get; set; }

            [JsonProperty("email", Required = Required.Always)]
            public string Email { get; set; }

            [JsonProperty("timestamp", Required = Required.Always)]
            public long Timestamp { get; set; }

            [JsonProperty("firstName")]
            public String FirstName { get; set; }

            [JsonProperty("lastName")]
            public String LastName { get; set; }

            [JsonProperty("displayName")]
            public String DisplayName { get; set; }

            [JsonProperty("profileImage")]
            public String ProfileImage { get; set; }
        }
    }
}
