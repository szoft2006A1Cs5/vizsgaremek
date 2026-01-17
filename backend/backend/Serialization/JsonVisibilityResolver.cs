using backend.Contexts;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace backend.Serialization
{
    public class JsonVisibilityResolver : DefaultJsonTypeInfoResolver
    {
        private readonly User? _user;
        private readonly Context _context;

        public JsonVisibilityResolver(User? user, Context ctx)
        {
            _context = ctx;
            _user = user;
        }

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo typeInfo = base.GetTypeInfo(type, options);

            if (typeInfo.Kind == JsonTypeInfoKind.Object)
            {
                var visKey = type
                    .GetProperties()
                    .FirstOrDefault(x => x.GetCustomAttribute<VisibilityKey>(true) != null);

                foreach (var prop in typeInfo.Properties)
                {
                    var propInfo = type.GetProperty(prop.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null) continue;

                    var attribute = propInfo.GetCustomAttribute<VisibleToAttribute>(true);
                    if (attribute == null) continue;

                    prop.ShouldSerialize = (obj, value) =>
                    {
                        if (attribute.VisibilityLevel == VisibilityLevel.Public) return true;
                        if (_user != null && _user.Role == UserRole.Administrator) return true;

                        if (visKey == null || _user == null) return false;

                        var visKeyVal = (int?)visKey.GetValue(obj);

                        var user = _context.Users
                            .Include(x => x.Rentals)
                            .ThenInclude(x => x.Vehicle)
                            .FirstOrDefault(x => x.Id == _user.Id);

                        if (user == null) return false;

                        if (user != null &&
                            visKeyVal != null &&
                            0 < user.Rentals.Count(x => x.RenterId == visKeyVal || x.Vehicle.OwnerId == visKeyVal) &&
                            attribute.VisibilityLevel <= VisibilityLevel.InRelation) return true;

                        if (user != null &&
                            visKeyVal != null &&
                            user.Id == visKeyVal &&
                            attribute.VisibilityLevel <= VisibilityLevel.OwnerOnly) return true;

                        return false;
                    };
                }
            }

            return typeInfo;
        }
    }
}
