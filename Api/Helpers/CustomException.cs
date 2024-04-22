using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Api.Helpers
{
    public class IdentityException : Exception
    {
        public List<string> Errors { get; set; }

        
        //public IdentityException(AuthenticationResult result)
        //{
        //    this.Errors = result.Errors.ToList();
        //}

        public IdentityException(IdentityResult result)
        {
            this.Errors = result.Errors.Select(x => x.Description).ToList();
        }
        public class InformativeException : Exception
        {
            public string InformativeMsg { get; set; }

            public InformativeException(string msg)
            {
                this.InformativeMsg = msg;
            }
        }

        public class Unauthorized : Exception
        {
            public string InformativeMsg { get; set; }

            public Unauthorized(string msg)
            {
                this.InformativeMsg = msg;
            }
        }
    }
}
