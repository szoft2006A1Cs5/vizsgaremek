using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using backend.Models;
using backend.Contexts;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.VisibilityFiltering
{
    public static class VisibilityFilterer
    {
        // AsyncLocal nagyon meno
        public static AsyncLocal<User?> AuthUser { get; } = new();
        private static JsonSerializerOptions SerializerOptions { get; } = new()
        {
            TypeInfoResolver = new JsonVisibilityResolver(),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        
        /// <summary>
        /// Vegigmegy az objektumon, es gyermekobjektumain, es JSON szerializalja,
        /// de sajatos TypeInfoResolverrel, igy csak az az informacio adodik vissza
        /// az IFilterable objektumokbol, amit a megadott felhasznalo lathat.
        /// </summary>
        /// <param name="data">A szerializalni es megszurni kivant objektum</param>
        /// <param name="authUser">Az felhasznalo, akinek megszurjuk az adatokat</param>
        /// <typeparam name="T">Barmilyen tipus/objektum szurheto</typeparam>
        /// <returns>A szerializalt (es megszurt) objektum string</returns>
        public static string FilterSerialize<T>(this T data, User? authUser)
        {
            AuthUser.Value = authUser;
            return JsonSerializer.Serialize(data, SerializerOptions);
        }
    }
}
