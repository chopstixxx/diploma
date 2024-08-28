using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace testmobile
{
    public class hashing
    {
        
        public string Hash_password(string input)
        {
            MD5 md5 = MD5.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (var a in hash)
                sb.Append(a.ToString("X2"));

            return sb.ToString();
        }
    }
}
