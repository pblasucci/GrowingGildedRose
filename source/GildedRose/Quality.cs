using System;
using static System.Math;

namespace GildedRose;

/// The value of a Ordinary item (n.b. constrained within: 0 .. 50, inclusive).
public readonly record struct Quality : IComparable<Quality>
{
    private readonly int value;

    /// Constructs a Quality from the given value
    /// (n.b. overlarge inputs are capped at Quality.MaxValue
    /// and undervalued inputs are lifted to Quality.MinValue).
    public Quality(int value)
    {
        this.value = Min(Max(value, 0), 50);
    }

    /// The smallest possible value of a Quality (0 units).
    public static Quality MinValue = new();

    /// The largest possible value of a Quality (50 units).
    public static Quality MaxValue = new(50);

    /// Defines an explicit conversion of a Quality to an signed 32-bit integer.
    public static explicit operator int(Quality quality) => quality.value;

    /// Overloaded addition operator
    public static Quality operator +(Quality left, Quality right)
    {
        var sum = left.value + right.value;
        return sum < left.value ? MaxValue : new Quality(sum);
    }

    /// Overloaded addition operator
    public static Quality operator +(Quality left, int right)
    {
        var sum = left.value + right;
        return sum < left.value ? MaxValue : new Quality(sum);
    }

    /// Overloaded subtraction operator
    public static Quality operator -(Quality left, Quality right)
    {
        var dif = left.value - right.value;
        return left.value < dif ? MinValue : new Quality(dif);
    }

    /// Overloaded subtraction operator
    public static Quality operator -(Quality left, int right)
    {
        var dif = left.value - right;
        return left.value < dif ? MinValue : new Quality(dif);
    }

    /// <inheritdoc />
    public int CompareTo(Quality other) => value.CompareTo(other.value);

    /// Overloaded comparison operator
    public static bool operator <(Quality left, Quality right) =>
        left.CompareTo(right) < 0;

    /// Overloaded comparison operator
    public static bool operator >(Quality left, Quality right) =>
        0 < left.CompareTo(right);

    /// Overloaded comparison operator
    public static bool operator <=(Quality left, Quality right) =>
        left.CompareTo(right) <= 0;

    /// Overloaded comparison operator
    public static bool operator >=(Quality left, Quality right) =>
        0 <= left.CompareTo(right);
}

/// The value of a Legendary item.
public readonly struct MagicQuality
{
    public static explicit operator int(MagicQuality _) => 80;
}
