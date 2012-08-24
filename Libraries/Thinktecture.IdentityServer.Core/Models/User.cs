using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinktecture.IdentityServer.Models
{
    public class User
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string SecurityQuestion { get; set; }

        public string SecurityAnswer{ get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
