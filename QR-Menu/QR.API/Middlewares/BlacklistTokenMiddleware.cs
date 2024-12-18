
//using HR.Application.Helpers;
//using HR_System.Application.Services;

//namespace HR.API.Middleware
//{
//    public class BlacklistTokenMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public BlacklistTokenMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }
//        public async Task InvokeAsync(HttpContext context)
//        {
//            var authHeader = context.Request.Headers["Authorization"].ToString();

//            if (!string.IsNullOrEmpty(authHeader))
//            {
//                var token = authHeader.Substring("Bearer ".Length).Trim();

//                var userClaimes = TokenHelper.ToUserClaims(token);

//                if (BlackListTokenService.isBlackListed(userClaimes.TokenId, userClaimes.ExpDate))
//                {
//                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//                    await context.Response.WriteAsync("Access forbidden: You are blacklisted.");
//                    return;
//                }
//            }
//            await _next(context);
//        }

//    }
//}
