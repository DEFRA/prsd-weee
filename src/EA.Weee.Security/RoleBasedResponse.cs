namespace EA.Weee.Security
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class RoleBasedResponse
    {
        private readonly IList<Role> roles;

        protected RoleBasedResponse()
        {
            roles = new List<Role>();
        }

        internal void AddUserRole(Role role)
        {
            roles.Add(role);
        }

        public bool DoesUserHaveRole(Roles role)
        {
            return roles
                .Select(r => r.Name)
                .Contains(role.ToString());
        }
    }
}
