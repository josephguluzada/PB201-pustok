using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pustok.Business.Exceptions.AuthorExceptions
{
    public class AuthorFullnameException : Exception
    {
        public string PropertyName { get; set; }
        public AuthorFullnameException()
        {
        }

        public AuthorFullnameException(string? message) : base(message)
        {
        }

        public AuthorFullnameException(string propertyName,string? message) : base(message)
        {
            PropertyName = propertyName;
        }
    }
}
