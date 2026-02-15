using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using backend.Models;
using backend.Contexts;
using System.Net;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;

namespace backend.VisibilityFiltering
{
    public static class VisibilityFilterer
    {
        public static AsyncLocal<User?> AuthUser { get; } = new();
        private static JsonSerializerOptions SerializerOptions { get; } = new()
        {
            TypeInfoResolver = new JsonVisibilityResolver(),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        
        public static string FilterVisibility<T>(this T data, User? authUser)
        {
            AuthUser.Value = authUser;
            return JsonSerializer.Serialize(data, SerializerOptions);
        }
    }
}
