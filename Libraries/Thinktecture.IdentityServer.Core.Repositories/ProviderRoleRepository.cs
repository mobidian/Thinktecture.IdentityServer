using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Thinktecture.IdentityServer.Repositories
{
    public class ProviderRoleRepository : IRoleRepository
    {
        public IEnumerable<string> GetRoles()
        {
            return Roles.GetAllRoles();
        }
    }
}
