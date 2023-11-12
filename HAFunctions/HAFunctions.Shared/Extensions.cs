using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace HAFunctions.Shared;

public static class Extensions
{
    public static async Task<ApiResultMessage> Call(this object obj, dynamic data = null, dynamic target = null)
    {
        if (obj is HomeAssistantService service)
        {
            return await service.Call(data, target);
        }
        return null;
    }

    public static string ToPascalCase(this string str)
    {
        var yourString = str.ToLower().Replace("_", " ");
        TextInfo info = CultureInfo.CurrentCulture.TextInfo;
        return info.ToTitleCase(yourString).Replace(" ", string.Empty);
    }

    public static string ToSnakeCase(this string str)
    {
        return Regex.Replace(Regex.Replace(str, "(.)([A-Z][a-z]+)", "$1_$2"), "([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    public static string GetDomain(this string str)
    {
        return str.Substring(0, str.IndexOf("."));
    }

    public static string GetEntityIdWithoutDomain(this string str)
    {
        return str.Substring(str.IndexOf(".") + 1, str.Length - str.IndexOf(".") - 1);
    }
}