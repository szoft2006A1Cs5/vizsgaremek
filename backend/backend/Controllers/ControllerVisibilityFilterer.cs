using backend.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using backend.Models;
using backend.Contexts;
using System.Net;

namespace backend.Controllers
{
    public static class ControllerVisibilityFilterer
    {
        public static ContentResult VisibilityTo<T>(T data, Tuple<int, UserRole, List<int>>? user, int statusCode)
        {
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new JsonVisibilityResolver(user),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var content = JsonSerializer.Serialize(data, options);

            return new ContentResult
            {
                StatusCode = statusCode,
                Content = content,
                ContentType = "application/json"
            };
        }
    }
}
