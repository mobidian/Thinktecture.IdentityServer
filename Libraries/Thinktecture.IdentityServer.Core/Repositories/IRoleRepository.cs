﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinktecture.IdentityServer.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<string> GetRoles();
    }
}
