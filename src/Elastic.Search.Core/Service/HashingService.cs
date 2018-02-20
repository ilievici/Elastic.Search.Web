using System;
using Elastic.Search.Core.Service.Abstract;
using System.Security.Cryptography;
using System.Text;

namespace Elastic.Search.Core.Service
{
    public class HashingService : IHashingService
    {
        /// <summary>
        /// Hash string
        /// </summary>
        public string HashString(string inputkey)
        {
            if (string.IsNullOrWhiteSpace(inputkey))
            {
                return string.Empty;
            }

            using (var sha = new SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(inputkey);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
