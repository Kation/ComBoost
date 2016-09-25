using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.SingleSignOn
{
    public class SingleSignOnDomain : DomainService
    {
        public SingleSignOnDomain(SingleSignOnCryptography cryptography)
        {
            if (cryptography == null)
                throw new ArgumentNullException(nameof(cryptography));
            Cryptography = cryptography;
        }

        public SingleSignOnCryptography Cryptography { get; private set; }

        public Task<bool> SignIn(IValueProvider valueProvider, IAuthenticationProvider authenticationProvider)
        {
            var dataString = valueProvider.GetRequriedValue<string>("data");
            var data = Convert.FromBase64String(dataString);
            data = Cryptography.Decrypt(data);

            var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(data));
            return authenticationProvider.SignInAsync(dict);
        }


    }
}
