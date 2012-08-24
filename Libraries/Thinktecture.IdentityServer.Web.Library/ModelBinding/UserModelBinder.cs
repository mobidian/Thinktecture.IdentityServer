using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.Web.ViewModels.Administration;

namespace Thinktecture.IdentityServer.Web.ModelBinding
{
    public class UserModelBinder : DefaultModelBinder
    {
        [Import]
        public IRoleRepository RoleRepository { get; set; }

        public UserModelBinder()
        {
            Container.Current.SatisfyImportsOnce(this);            
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            UserModel model = (UserModel) bindingContext.Model ?? new UserModel();

            model.UserName = GetValue(bindingContext, "UserName");
            model.Password = GetValue(bindingContext, "Password");
            model.Email = GetValue(bindingContext, "Email");
            model.SecurityQuestion = GetValue(bindingContext, "SecurityQuestion");
            model.SecurityAnswer = GetValue(bindingContext, "SecurityAnswer");

            IList<string> roles = new List<string>();
            foreach(string roleName in RoleRepository.GetRoles())
            {
                bool isChecked = IsChecked(bindingContext, string.Format("roles.{0}", roleName));

                if(isChecked)
                    roles.Add(roleName);
            }

            model.Roles = roles;

            return model;
        }

        private string GetValue(ModelBindingContext context, string key)
        {
            ValueProviderResult vpr = context.ValueProvider.GetValue(key);
            return vpr == null ? null : vpr.AttemptedValue;
        }

        private bool IsChecked(ModelBindingContext bindingContext, string key)
        {
            ValueProviderResult result = bindingContext.ValueProvider.GetValue(key);
            if(result != null)
                return (bool)result.ConvertTo(typeof (bool));
            else
                return false;
        }
    }
}
