using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Organizations;
using Abp.Timing;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.UserDelegations;
using Eaf.Workflow.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.ProjectName.Workflows
{
    /// <summary>
    /// Resolves workflow assignee kinds against the real user directory:
    /// role members, organization-unit members, requester's manager and active delegations.
    /// </summary>
    public class WorkflowUserDirectory : IWorkflowUserDirectory, ITransientDependency
    {
        private readonly IRepository<User, long> _users;
        private readonly IRepository<Role> _roles;
        private readonly IRepository<UserRole, long> _userRoles;
        private readonly IRepository<OrganizationUnit, long> _organizationUnits;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnits;
        private readonly IRepository<UserDelegation, long> _userDelegations;

        /// <summary>WorkflowUserDirectory.</summary>
        public WorkflowUserDirectory(
            IRepository<User, long> users,
            IRepository<Role> roles,
            IRepository<UserRole, long> userRoles,
            IRepository<OrganizationUnit, long> organizationUnits,
            IRepository<UserOrganizationUnit, long> userOrganizationUnits,
            IRepository<UserDelegation, long> userDelegations)
        {
            _users = users;
            _roles = roles;
            _userRoles = userRoles;
            _organizationUnits = organizationUnits;
            _userOrganizationUnits = userOrganizationUnits;
            _userDelegations = userDelegations;
        }

        /// <inheritdoc />
        public async Task<List<long>> GetUsersInRoleAsync(string roleName, int? tenantId)
        {
            var roleIds = await _roles.GetAllListAsync(r => r.Name == roleName && r.TenantId == tenantId);
            var ids = roleIds.Select(r => r.Id).ToList();
            if (ids.Count == 0)
            {
                return new List<long>();
            }
            var userRoles = await _userRoles.GetAllListAsync(ur => ids.Contains(ur.RoleId) && ur.TenantId == tenantId);
            return userRoles.Select(ur => ur.UserId).Distinct().ToList();
        }

        /// <inheritdoc />
        public async Task<List<long>> GetUsersInOrganizationUnitAsync(string ouCode, int? tenantId)
        {
            var ous = await _organizationUnits.GetAllListAsync(o => o.Code == ouCode && o.TenantId == tenantId);
            var ids = ous.Select(o => o.Id).ToList();
            if (ids.Count == 0)
            {
                return new List<long>();
            }
            var members = await _userOrganizationUnits.GetAllListAsync(uo => ids.Contains(uo.OrganizationUnitId) && uo.TenantId == tenantId);
            return members.Select(uo => uo.UserId).Distinct().ToList();
        }

        /// <inheritdoc />
        public async Task<long?> GetManagerAsync(long userId, int? tenantId)
        {
            var user = await _users.FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId);
            return user?.ManagerUserId;
        }

        /// <inheritdoc />
        public async Task<List<long>> GetActiveDelegatorIdsAsync(long delegateUserId, int? tenantId)
        {
            var now = Clock.Now;
            var delegations = await _userDelegations.GetAllListAsync(d =>
                d.TargetUserId == delegateUserId &&
                d.TenantId == tenantId &&
                d.StartTime <= now &&
                d.EndTime >= now);
            return delegations.Select(d => d.SourceUserId).Distinct().ToList();
        }
    }
}
