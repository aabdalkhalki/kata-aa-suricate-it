namespace Library.Application;

public static class Clock
{
    extension(TimeProvider timeProvider)
    {
        public DateOnly Today() => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
    }
}
