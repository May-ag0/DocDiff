using System.Globalization;

namespace DocDiff.Web.Components;

public static class DisplayFormat
{
    /// <summary>Formats a UTC timestamp in the server's local time, e.g. "28 Sep 2026, 14:54".</summary>
    public static string LocalTime(DateTime utc) =>
        utc.ToLocalTime().ToString("d MMM yyyy, HH:mm", CultureInfo.InvariantCulture);
}
