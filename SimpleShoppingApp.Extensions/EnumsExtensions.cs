using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SimpleShoppingApp.Extensions
{
    public static class EnumsExtension
    {
        public static string GetDisplayName(this Enum value)
        {
            var displayName = value
              .GetType()
              .GetField(value.ToString())?
              .GetCustomAttribute<DisplayAttribute>()?
              .Name;

            if (displayName == null)
            {
                return value.ToString();
            }
            return displayName;
        }
    }
}
