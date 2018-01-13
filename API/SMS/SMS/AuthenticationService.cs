using Microsoft.AspNetCore.Http;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS
{
    public class AuthenticationService
    {
        private readonly RequestDelegate _next;
        private readonly PlivoCodingAssignmentContext _context;

        public AuthenticationService(RequestDelegate next, PlivoCodingAssignmentContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string authHeader = httpContext.Request.Headers["Authorization"];
            string method = httpContext.Request.Method;
            if (method.ToLower().Equals("post"))
            {
                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    string userName = encodedUsernamePassword.Split(':')[0];
                    string password = encodedUsernamePassword.Split(':')[1];

                    Account account = _context.Account.Where(x => x.Username == userName).FirstOrDefault();
                    if (account != null)
                    {
                        if (account.AuthId.Equals(password))
                        {
                            await _next.Invoke(httpContext);
                        }
                        else
                        {
                            httpContext.Response.StatusCode = 403;
                            return;
                        }
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 403;
                        return;
                    }
                }
                else
                {
                    httpContext.Response.StatusCode = 403;
                    return;
                }
            }
            else
            {
                httpContext.Response.StatusCode = 405;
                return;
            }
        }
    }
}
