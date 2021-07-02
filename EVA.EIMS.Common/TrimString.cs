using System.Linq;

namespace EVA.EIMS.Common
{
    public class TrimString
    {
        public static object TrimObjectStringValue(object obj)
        {
            var stringProperties = obj.GetType().GetProperties()
                          .Where(p => p.PropertyType == typeof(string));

            foreach (var stringProperty in stringProperties)
            {
                var currentValue = (string)stringProperty.GetValue(obj, null);
                stringProperty.SetValue(obj, currentValue?.Trim(), null);
            }

            return obj;

        }
    }
}
