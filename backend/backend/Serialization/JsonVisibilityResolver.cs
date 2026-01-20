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

        public JsonVisibilityResolver(User? user)
        {
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
                        if (_user == null) return false;

                        if (_user.Role == UserRole.Administrator) return true;
                        if (visKey == null) return false;

                        var visKeyVal = (int?)visKey.GetValue(obj);
                        if (visKeyVal == null) return false;

                        if (0 < _user.Rentals.Count(x => x.RenterId == visKeyVal || x.Vehicle.OwnerId == visKeyVal) &&
                            attribute.VisibilityLevel <= VisibilityLevel.InRelation) return true;

                        if (_user.Id == visKeyVal &&
                            attribute.VisibilityLevel <= VisibilityLevel.OwnerOnly) return true;

                        return false;
                    };
                }
            }

            return typeInfo;
        }
    }
}
