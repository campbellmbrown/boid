using System;

namespace Boid.Utility;

public record Ref<T> where T : struct, IComparable<T>
{
    public T Value { get; set; }

    public Ref(T value)
    {
        Value = value;
    }

    public static bool operator >(Ref<T> left, T right) => left.Value.CompareTo(right) > 0;
    public static bool operator <(Ref<T> left, T right) => left.Value.CompareTo(right) < 0;
    public static bool operator >(T left, Ref<T> right) => left.CompareTo(right.Value) > 0;
    public static bool operator <(T left, Ref<T> right) => left.CompareTo(right.Value) < 0;
}
