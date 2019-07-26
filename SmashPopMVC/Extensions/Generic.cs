using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SmashPopMVC.Extensions
{
    public static class Generic
    {
        public static ViewDataDictionary ViewData { get; private set; }

        public static RouteValueDictionary AsRouteValueDictionary(this JsonResult jsonResult)
        {
            return new RouteValueDictionary(jsonResult.Value);
        }

        public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return await Task.WhenAll(tasks);
        }
    }
}
