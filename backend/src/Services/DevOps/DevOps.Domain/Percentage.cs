using Domain;

public class Percentage : ValueObject<Percentage>
{
    private readonly double _value;

    public Percentage(double value)
    {
        _value = value;
        if (value < 0 || value > 1)
        {
            throw new ContractException("Percentage value must be between 0 and 1.");
        }

    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
    public static implicit operator double (Percentage percentage)
    {
        return percentage._value;
    }

    public static implicit operator Percentage(double value)
    {
        return new Percentage(value);
    }
    public static implicit operator string(Percentage percentage)
    {
        return percentage.ToString();
    }

    public override string ToString()
    {
        return $"{_value * 100:0.##}%";
    }
}