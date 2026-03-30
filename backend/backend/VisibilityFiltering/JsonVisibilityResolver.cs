using backend.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using backend.VisibilityFiltering;

namespace backend.VisibilityFiltering
{
    public class JsonVisibilityResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo typeInfo = base.GetTypeInfo(type, options);

            if (!typeof(IFilterable).IsAssignableFrom(type))
                return typeInfo;
            
            var getVisCondMethod = type.GetMethod("GetVisibilityConditionLambda", BindingFlags.Public | BindingFlags.Static);
            if (getVisCondMethod == null) return typeInfo;
            
            if (typeInfo.Kind == JsonTypeInfoKind.Object)
            {
                foreach (var prop in typeInfo.Properties)
                {
                    var propInfo = type.GetProperty(prop.Name, BindingFlags.IgnoreCase | BindingFlags.Public 
                        | BindingFlags.Instance);
                    if (propInfo == null) continue;

                    var attribute = propInfo.GetCustomAttribute<VisibleToAttribute>(true);
                    if (attribute == null) continue;

                    var visCondLambda = getVisCondMethod.Invoke(null, [attribute.VisibilityLevel])
                        as Func<object?, User?, bool>;
                    if (visCondLambda == null) continue;
                    
                    prop.ShouldSerialize =
                        (obj, _) => visCondLambda(obj, VisibilityFilterer.AuthUser.Value);
                }
            }

            return typeInfo;
        }
    }
}
