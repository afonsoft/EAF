using Abp.Authorization.Users;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.ProjectName.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Eaf.ProjectName.Migrations.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly ProjectNameDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(ProjectNameDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            //Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true, IsDefault = true }).Entity;
                _context.SaveChanges();
            }

            //admin user

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "projectname@afonsoft.com.br");
                adminUser.Surname = "projectname";
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;
                adminUser.ShouldChangePasswordOnNextLogin = true;
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();

                //User account of admin user
                if (_tenantId == 1)
                {
                    _context.UserAccounts.Add(new UserAccount
                    {
                        TenantId = _tenantId,
                        UserId = adminUser.Id,
                        UserName = AbpUserBase.AdminUserName,
                        EmailAddress = adminUser.EmailAddress
                    });
                    _context.SaveChanges();
                }

                _context.SaveChanges();
            }

            SeedWorkflowTemplate();
        }

        private void SeedWorkflowTemplate()
        {
            if (_context.WorkflowDefinitions.IgnoreQueryFilters().Any(d => d.TenantId == _tenantId))
            {
                return;
            }

            var form = new Eaf.Workflow.Domain.WorkflowFormModel();
            form.Fields.Add(new Eaf.Workflow.Domain.WorkflowFormField { Id = "reason", Label = "Reason", Type = "text", Required = true });
            form.Fields.Add(new Eaf.Workflow.Domain.WorkflowFormField { Id = "amount", Label = "Amount", Type = "number", Required = true });

            var flow = new Eaf.Workflow.Domain.WorkflowFlowModel { StartStepId = "approve" };
            flow.Steps.Add(new Eaf.Workflow.Domain.WorkflowStepModel
            {
                Id = "approve",
                Name = "Manager approval",
                Type = "approval",
                NextStepId = "end",
                Assignee = new Eaf.Workflow.Domain.WorkflowAssigneeModel { Kind = "manager" },
                DueTimeHours = 48
            });
            flow.Steps.Add(new Eaf.Workflow.Domain.WorkflowStepModel { Id = "end", Name = "Done", Type = "end" });

            var formJson = System.Text.Json.JsonSerializer.Serialize(form);
            var flowJson = System.Text.Json.JsonSerializer.Serialize(flow);
            var definitionId = Guid.NewGuid();

            _context.WorkflowDefinitions.Add(new Eaf.Workflow.Domain.WorkflowDefinition
            {
                Id = definitionId,
                TenantId = _tenantId,
                Name = "Expense approval (sample)",
                Description = "Sample published workflow — manager approves expenses submitted through the catalog.",
                Category = "Samples",
                Status = Eaf.Workflow.Domain.WorkflowStatus.Published,
                Version = 1,
                FormJson = formJson,
                FlowJson = flowJson
            });
            _context.WorkflowVersions.Add(new Eaf.Workflow.Domain.WorkflowVersion
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                DefinitionId = definitionId,
                Version = 1,
                FormJson = formJson,
                FlowJson = flowJson
            });
            _context.SaveChanges();
        }
    }
}