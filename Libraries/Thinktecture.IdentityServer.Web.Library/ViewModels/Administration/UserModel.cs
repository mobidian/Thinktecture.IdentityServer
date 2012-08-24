using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Thinktecture.IdentityServer.Web.ViewModels.Administration
{
    public class UserModel
    {
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        [DisplayName("Security Question")]
        public string SecurityQuestion { get; set; }

        [DisplayName("Security Answer")]
        public string SecurityAnswer { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
