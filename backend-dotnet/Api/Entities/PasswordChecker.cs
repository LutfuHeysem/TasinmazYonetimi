using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Api.Entities
{
    public class PasswordChecker
    {
        public static string Sifreleme(string sifre)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(sifre));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string SifreKontrol(string sifre)
        {
            string stregex = @"^[a-zA-Z0-9#?!@$%^&*+./]{8,20}$";
            Regex rgx = new Regex(stregex);
            if (rgx.IsMatch(sifre))
            {
                return sifre;
            }
            else
            {
                return "hata";
            }
        }

        public static bool IsPasswordCompliant(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8 || password.Length > 20)
            {
                return false;
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(ch));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
    }
}