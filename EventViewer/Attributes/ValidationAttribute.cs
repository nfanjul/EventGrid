using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Linq;

namespace EventViewer.Attributes
{
    public class ValidationAttribute : Attribute, IActionConstraint
    {
        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.Headers.Any(header => header.Key == "Aeg-Event-Type" && header.Value == "SubscriptionValidation");
        }
    }
}
