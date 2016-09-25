using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.SingleSignOn
{
    public class RSACryptography : SingleSignOnCryptography
    {
        public RSACryptography()
        {

        }

        public RSA RSA { get; private set; }

        public override byte[] Decrypt(byte[] data)
        {
            List<byte> result = new List<byte>();
            var size = RSA.KeySize / 8 - 11;
            for (int i = 0; i < data.Length; i += size)
            {
                byte[] item = data.Skip(i).Take(size).ToArray();
#if NET451
                result.AddRange(RSA.DecryptValue(item));
#else
                result.AddRange(RSA.Decrypt(item, RSAEncryptionPadding.Pkcs1));
#endif
            }
            return result.ToArray();
        }

        public override byte[] Encrypt(byte[] data)
        {
            List<byte> result = new List<byte>();
            var size = RSA.KeySize / 8 -11;
            for (int i = 0; i < data.Length; i += size)
            {
                byte[] item = data.Skip(i).Take(size).ToArray();
#if NET451
                result.AddRange(RSA.EncryptValue(item));
#else
                result.AddRange(RSA.Encrypt(item, RSAEncryptionPadding.Pkcs1));
#endif
            }
            return result.ToArray();
        }
    }
}
