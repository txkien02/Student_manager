namespace Web_Student_manager.Models
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    public class UserRoleRequirement : IAuthorizationRequirement
    {
        public string RequiredRole { get; }

        public UserRoleRequirement(string requiredRole)
        {
            RequiredRole = requiredRole;
        }
    }

    public class UserRoleHandler : AuthorizationHandler<UserRoleRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRoleHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRoleRequirement requirement)
        {
            var userRole = _httpContextAccessor.HttpContext.Session.GetString("UserRole");

            if (!string.IsNullOrEmpty(userRole) && userRole == requirement.RequiredRole)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
