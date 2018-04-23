using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading.Tasks;

namespace EventViewer.AuthorizationPolicy
{
    public class KeyHandler : AuthorizationHandler<KeyRequirement>
    {
        private readonly IActionContextAccessor _accessor;

        public KeyHandler(IActionContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, KeyRequirement requirement)
        {
            if (_accessor.ActionContext.HttpContext.Request.Query.TryGetValue("key", out StringValues values)
                && values.Count() == 1
                && values.Single().ToUpperInvariant() == requirement.Key.ToUpperInvariant())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}