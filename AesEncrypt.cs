using System;
using System.IO;
using System.Security.Cryptography;


public static string Encrypt(string str) {
    
    using(var aes = new AesManaged()) {
        HashAlgorithm algorithm = SHA1.Create();
        aes.Key = algorithm.ComputeHash(Encoding.UTF8.GetBytes(secretKey)).Take(16).ToArray();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;

        using(var ms = new MemoryStream()) {
            using(var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                byte[] data = Encoding.UTF8.GetBytes(str);
                cs.Write(data, 0, data.Length);
            }

            byte[] encrypted = ms.ToArray();
            return Convert.ToBase64String(encrypted).Replace('+', '-').Replace('/', '_').Replace("=", "");
        }
    }
}