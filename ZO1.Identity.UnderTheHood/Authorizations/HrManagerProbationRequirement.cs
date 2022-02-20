using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DateTime = System.DateTime;

namespace ZO1.Identity.UnderTheHood.Authorizations
{
    public class HrManagerProbationRequirement : IAuthorizationRequirement
    {
        public int ProbationMonths { get; }

        public HrManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }
    }


    public class HrManagerProbationRequirementHandler : AuthorizationHandler<HrManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            HrManagerProbationRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "EmploymentDate"))
                return Task.CompletedTask;

            var empDate = DateTime.Parse(context.User.FindFirst(x =>
                        x.Type == "EmploymentDate")?.Value ?? string.Empty);

            var period = DateTime.Now - empDate;
            if (period.Days > 30 * requirement.ProbationMonths)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}