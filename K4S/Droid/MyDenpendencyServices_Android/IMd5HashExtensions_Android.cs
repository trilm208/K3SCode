using System;
using System.Data;
using LoginXF.Droid;
using Xamarin.Forms;
using System.Security.Cryptography;
using System.Text;
using MyDependencyServices;

[assembly: Dependency(typeof(Md5HashExtensions_Android))]
namespace LoginXF.Droid
{
    public class Md5HashExtensions_Android : IMd5HashExtensions
    {
        public  string GetMd5Hash( byte[] input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(input);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public  bool VerifyMd5Hash( byte[] input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public  string GetMd5Hash( string input)
        {
            return GetMd5Hash(Encoding.Default.GetBytes(input));
        }

        public  bool VerifyMd5Hash( string input, string hash)
        {
            return VerifyMd5Hash(Encoding.Default.GetBytes(input), hash);
        }
    }
}
