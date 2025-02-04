using Microsoft.AspNetCore.Builder;

namespace UserManagement.Application.Extensions;

public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static void UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
