namespace Library.Domain;

public readonly record struct Money(decimal Amount)
{
    public static Money Zero => new(0m);

    public static Money operator *(Money money, int multiplier) => new(money.Amount * multiplier);

    public static Money Min(Money left, Money right) => left.Amount <= right.Amount ? left : right;
}
