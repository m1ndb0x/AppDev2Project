using AppDev2Project.Models;
using Microsoft.AspNetCore.Identity;

namespace AppDev2Project.Middleware
{
    public class UserActivityMiddleware
    {
        private readonly RequestDelegate _next;

        public UserActivityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, ExaminaDatabaseContext dbContext)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    user.LastActivity = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }
            }

            await _next(context);
        }
    }

    public static class UserActivityMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserActivity(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserActivityMiddleware>();
        }
    }
}
