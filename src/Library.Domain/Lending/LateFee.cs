namespace Library.Domain.Lending;

public static class LateFee
{
    public static Money DailyRate { get; } = new(0.20m);

    public static Money Cap { get; } = new(10.00m);

    public static Money For(int daysLate)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(daysLate);

        return Money.Min(DailyRate * daysLate, Cap);
    }
}
