using EVA.EIMS.Helper;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Web;

namespace EVA.EIMS.Common
{
	/// <summary>
	/// Generates a random encrypted string as OTP.
	/// </summary>
	public static class CommonMethods
	{
        private static Random random = new Random();
        public static string RandomString()
        {
            var bytes = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            return HttpUtility.UrlEncode(Convert.ToBase64String(bytes));
        }

        /// <summary>
        /// Generate random otp with integer
        /// </summary>
        /// <param name="oTPLength"></param>
        /// <returns></returns>
        public static string RandomOTP(int oTPLength)
        {
            string[] allowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string otp = String.Empty;
            string tempChars = String.Empty;
            Random rand = new Random();
            for (int i = 0; i < oTPLength; i++)
            {
                int p = rand.Next(0, allowedCharacters.Length);
                tempChars = allowedCharacters[rand.Next(0, allowedCharacters.Length)];
                otp += tempChars;
            }
            return otp;
        }
	}
}
