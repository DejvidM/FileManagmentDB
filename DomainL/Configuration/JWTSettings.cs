using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainL.Configuration
{
    public class JWTSettings
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SecretKey { get; set; }
        public int TokenLifetime { get; set; } 
    }
}
