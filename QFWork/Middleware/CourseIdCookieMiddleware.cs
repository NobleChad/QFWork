namespace QFWork.Middleware
{
    public class CourseIdCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public CourseIdCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            if (!context.Request.Cookies.ContainsKey("CourseId") && path.HasValue && path.Value.Contains("/Course"))
            {
                var segments = path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 3 && int.TryParse(segments[2], out int courseId))
                {
                    context.Response.Cookies.Append("CourseId", courseId.ToString(), new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddHours(1),
                        HttpOnly = true,
                        Secure = true
                    });
                }
            }

            await _next(context);
        }
    }

}
