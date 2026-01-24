using backend.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using backend.Models;
using backend.Contexts;

namespace backend.Controllers
{
    public static class ControllerVisibilityFilterer
    {
        public static JsonResult VisibilityTo<T>(T data, Tuple<int, UserRole, List<int>>? user)
        {
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new JsonVisibilityResolver(user),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var result = new JsonResult(data, options);
            result.StatusCode = 200;

            return result;
        }
    }
}
