using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data.Enums
{

    public enum ValidationsType
    {
        None = 0,
        EntityId = 1,
        //Obj = 2,
    }

    public enum ErrorType
    {
        None = 0,
        Argument,
        Informative,
        Exception
    }
     
    public enum UserRole
    {
        Admin,
        Customer
    }
     
}
