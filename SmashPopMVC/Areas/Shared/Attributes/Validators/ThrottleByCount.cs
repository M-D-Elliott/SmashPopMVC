using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmashPopMVC.Areas.Shared.Attributes.Validators
{
    public enum TimeUnit
    {
        Minute = 60,
        Hour = 3600,
        Day = 86400
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ThrottleByCountAttribute : ActionFilterAttribute
    {
        public TimeUnit TimeUnit { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

        private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());

        public override void OnActionExecuting(ActionExecutingContext c)
        {
            var seconds = Convert.ToInt32(TimeUnit);

            var key = string.Join(
                Name,
                "-",
                c.HttpContext.Request.HttpContext.Connection.RemoteIpAddress
            );

            int countSet = 1;
            if (Cache.TryGetValue(key, out int cnt))
            {
                countSet = cnt + 1;
                if (cnt >= Count)
                {
                    c.Result = new ContentResult
                    {
                        Content = "You are allowed to " + Message + " " + Count + " times per " + TimeUnit.ToString().ToLower()
                    };
                    c.HttpContext.Response.StatusCode = 429;
                }

            }
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(seconds));

            Cache.Set(key, countSet, cacheEntryOptions);
        }
    }

}
