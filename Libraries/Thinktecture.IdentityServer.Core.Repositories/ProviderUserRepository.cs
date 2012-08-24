/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Profile;
using System.Web.Security;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityServer.Models;
using Thinktecture.IdentityServer.TokenService;

namespace Thinktecture.IdentityServer.Repositories
{
    public class ProviderUserRepository : IUserRepository
    {
        [Import]
        public IClientCertificatesRepository Repository { get; set; }

        public ProviderUserRepository()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public bool ValidateUser(string userName, string password)
        {
            return Membership.ValidateUser(userName, password);
        }

        public bool ValidateUser(X509Certificate2 clientCertificate, out string userName)
        {
            return Repository.TryGetUserNameFromThumbprint(clientCertificate, out userName);
        }

        public IEnumerable<string> GetRoles(string userName)
        {
            var returnedRoles = new List<string>();

            if (Roles.Enabled)
            {
                var roles = Roles.GetRolesForUser(userName);
                returnedRoles = roles.Where(role => role.StartsWith(Constants.Roles.InternalRolesPrefix)).ToList();    
            }

            return returnedRoles;
        }

        public void Add(User user)
        {
            MembershipCreateStatus createStatus;

            Membership.CreateUser(user.UserName, user.Password, user.Email, user.SecurityQuestion,
                user.SecurityAnswer, true, out createStatus);

            if(createStatus != MembershipCreateStatus.Success)
                throw new Exception(string.Format("Error when creating user: {0}.", createStatus));
            else
            {
                Roles.AddUserToRoles(user.UserName, user.Roles.ToArray());
            }
        }

        public void Edit(User user)
        {
            MembershipUser membershipUser = Membership.GetUser(user.UserName);
            if(membershipUser != null)
            {
                membershipUser.Email = user.Email;
                Membership.UpdateUser(membershipUser);

                string[] newRoles = user.Roles.ToArray();
                string[] currentRoles = Roles.GetRolesForUser(user.UserName);

                string[] rolesToDelete = currentRoles.Except(newRoles).ToArray();
                string[] rolesToAdd = newRoles.Except(currentRoles).ToArray();

                if(rolesToDelete.Length > 0)
                    Roles.RemoveUserFromRoles(user.UserName, rolesToDelete);

                if (rolesToAdd.Length > 0)
                    Roles.AddUserToRoles(user.UserName, rolesToAdd);
            }
            else
            {
                throw new Exception(string.Format("Unknown user '{0}'.", user.UserName));
            }
        }

        public void Delete(string userName)
        {
            Membership.DeleteUser(userName);
        }

        public User GetUser(string userName)
        {
            MembershipUser membershipUser = Membership.GetUser(userName);
            if (membershipUser != null)
            {
                return new User()
                {
                    UserName = membershipUser.UserName,
                    Email = membershipUser.Email,
                    SecurityQuestion = membershipUser.PasswordQuestion,
                    Roles = Roles.GetRolesForUser(membershipUser.UserName)
                };
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<User> GetUsers()
        {
            List<User> users = new List<User>();

            foreach (MembershipUser membershipUser in Membership.GetAllUsers())
            {
                users.Add(new User()
                {
                    UserName = membershipUser.UserName,
                    Roles = Roles.GetRolesForUser(membershipUser.UserName)
                });
            }

            return users;
        }
    }
}