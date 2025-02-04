namespace UserManagement.Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToDateTimeOrNow(this long? timestamp)
    {
        return timestamp.HasValue
            ? DateTimeOffset.FromUnixTimeMilliseconds(timestamp.Value).DateTime
            : DateTime.Now;
    }

    public static long ToUnixTimestamp(this DateTime? dateTime)
    {
        return dateTime.HasValue
            ? ((DateTimeOffset)dateTime.Value).ToUnixTimeSeconds()
            : ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
    }
}
