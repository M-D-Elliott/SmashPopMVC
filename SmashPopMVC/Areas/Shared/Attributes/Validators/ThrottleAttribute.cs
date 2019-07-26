using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;

namespace SmashPopMVC.Areas.Shared.Attributes.Validators
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ThrottleAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Seconds { get; set; }
        public string Message { get; set; }

        private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());

        public override void OnActionExecuting(ActionExecutingContext c)
        {
            var key = string.Concat(Name, "-", c.HttpContext.Request.HttpContext.Connection.RemoteIpAddress);

            if (!Cache.TryGetValue(key, out bool entry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));

                Cache.Set(key, true, cacheEntryOptions);
            }
            else 
            {
                if (string.IsNullOrEmpty(Message))
                    Message = "You may only perform this action every {n}.";
                TimeSpan t = TimeSpan.FromSeconds(Seconds);
                string time = t.Days > 0 ?
                    time = t.Days + " days"
                    : t.Hours > 0 ?
                    time = t.Hours + " hours"
                    : t.Minutes > 0 ?
                    time = t.Minutes + " minutes"
                    : t.Seconds + " seconds";
                c.Result = new ContentResult {Content = Message.Replace("{n}", time)};
                c.HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
            }
        }
    }

}
