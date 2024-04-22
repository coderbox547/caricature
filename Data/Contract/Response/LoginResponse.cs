using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contract.Response
{
    public class LoginResponse
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string UserLogo { get; set; }

        public string Name { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }

    }
}
