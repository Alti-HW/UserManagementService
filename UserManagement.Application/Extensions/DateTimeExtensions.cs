namespace UserManagement.Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToDateTimeOrNow(this long? timestamp)
    {
        return timestamp.HasValue
            ? DateTimeOffset.FromUnixTimeMilliseconds(timestamp.Value).DateTime
            : DateTime.Now;
    }
}
