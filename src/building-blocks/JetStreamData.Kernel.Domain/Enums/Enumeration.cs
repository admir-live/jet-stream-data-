using System.Reflection;

namespace JetStreamData.Kernel.Domain.Enums;

public abstract class Enumeration : IComparable
{
    protected Enumeration()
    {
    }

    protected Enumeration(
        int id,
        string name)
    {
        Id = id;
        Name = name;
    }

    public string Name { get; } = null!;
    public int Id { get; }

    public int CompareTo(object other)
    {
        return Id.CompareTo(((Enumeration)other)!.Id);
    }

    public override string ToString()
    {
        return Name;
    }

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    public override bool Equals(object obj)
    {
        var otherValue = obj as Enumeration;

        if (otherValue == null)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static int AbsoluteDifference(
        Enumeration firstValue,
        Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    private static T Parse<T, K>(
        K value,
        string description,
        Func<T, bool> predicate) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
        {
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
        }

        return matchingItem;
    }

    public static bool operator ==(
        Enumeration left,
        Enumeration right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        return left?.Equals(right) ?? false;
    }

    public static bool operator !=(
        Enumeration left,
        Enumeration right)
    {
        return !(left == right);
    }
}
