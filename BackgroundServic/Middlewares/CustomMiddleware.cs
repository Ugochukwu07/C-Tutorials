namespace BackgroundServic.Middlewares;

public static class CustomMiddleware
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
        
        return builder;
    }
}