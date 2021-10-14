using System;
using static System.Math;

namespace GildedRose
{
    /// The value of a Ordinary item (n.b. constrained within: 0 .. 50, inclusive).
    public readonly struct Quality
        : IEquatable<Quality>, IComparable<Quality>, IComparable
    {
        private readonly byte value;

        /// Constructs a Quality from the given value
        /// (n.b. overlarge inputs are capped at Quality.MaxValue
        /// and undervalued inputs are lifted to Quality.MinValue).
        public Quality(int value)
        {
            this.value = (byte) Min(Max(value, 0), 50);
        }

        /// <inheritdoc />
        public override string ToString() => $"{value:D2}";

        /// <inheritdoc />
        public override int GetHashCode() => value.GetHashCode();

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is Quality other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Quality other) => value == other.value;

        /// <inheritdoc />
        public int CompareTo(Quality other) => value.CompareTo(other.value);

        /// <inheritdoc />
        public int CompareTo(object? obj)
        {
            return obj switch
            {
                null => +1,
                Quality other => CompareTo(other),
                _ => throw new ArgumentException(
                        $"can only compare with {nameof(Quality)}",
                        nameof(obj)
                     )
            };
        }

        /// The smallest possible value of a Quality (0 units).
        public static Quality MinValue = new ();
        /// The largest possible value of a Quality (50 units).
        public static Quality MaxValue = new (50);

        /// Defines an explicit conversion of a Quality to an signed 32-bit integer.
        public static explicit operator int(Quality quality) => quality.value;

        public static bool operator ==
            (Quality left, Quality right) => left.Equals(right);

        public static bool operator !=
            (Quality left, Quality right) => !(left == right);

        public static bool operator <
            (Quality left, Quality right) => left.CompareTo(right) < 0;

        public static bool operator >
            (Quality left, Quality right) => 0 < left.CompareTo(right);

        public static bool operator <=
            (Quality left, Quality right) => left.CompareTo(right) <= 0;

        public static bool operator >=
            (Quality left, Quality right) => 0 <= left.CompareTo(right);

        /// Overloaded addition operator
        public static Quality operator +(Quality left, Quality right)
        {
            var sum = left.value + right.value;
            return sum < left.value ? MaxValue : new Quality(sum);
        }

        /// Overloaded subtraction operator
        public static Quality operator -(Quality left, Quality right)
        {
            var dif = left.value - right.value;
            return left.value < dif ? MinValue : new Quality(dif);
        }
    }

    /// the value of a Legendary item.
    public readonly struct MagicQuality
    {
        public static explicit operator int (MagicQuality _) => 80;
    }
}
