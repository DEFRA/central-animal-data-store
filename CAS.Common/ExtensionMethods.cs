namespace CAS.Common;

public static class ExtensionMethods
{
    /// <summary>
    /// Normalizes empty strings to null and trims valid strings
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Null or non-empty trimmed string</returns>
    public static string? Clean(this string? value) => string.IsNullOrWhiteSpace(value) ? null : value?.Trim();
}
