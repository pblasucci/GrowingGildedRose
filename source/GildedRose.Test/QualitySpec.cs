using FsCheck;
using FsCheck.Xunit;

namespace GildedRose.Test
{
    public sealed class QualitySpec
    {
        [Property]
        public Property Construction_truncates_low(NegativeInt input)
        {
            var raw = (int)input;
            Quality quality = new(raw);

            return (quality == Quality.MinValue)
                .Label($"{nameof(raw)}: {raw}, {nameof(quality)}: {quality}");
        }

        [Property]
        public Property Construction_truncates_high(PositiveInt input)
        {
            var raw = (int)input;
            Quality quality = new(raw);

            var doesHold = raw switch
            {
                < 50 => (int)quality == raw,
                _ => quality == Quality.MaxValue
            };

            return doesHold
                .Label($"{nameof(raw)}: {raw}, {nameof(quality)}: {quality}");
        }

        [Property]
        public Property Addition_truncates_high(
            PositiveInt input1,
            PositiveInt input2
        )
        {
            var (raw1, raw2) = ((int)input1, (int)input2);
            var raw3 = raw1 + raw2;

            Quality quality1 = new(raw1), quality2 = new(raw2);
            var quality3 = quality1 + quality2;

            return (raw3 >= raw1 || quality3 == Quality.MaxValue)
                .Label($"raw sum: {raw3}, quality sum: {quality3}");
        }

        [Property]
        public bool Additive_identity_holds(Quality quality)
            => quality + Quality.MinValue == quality;

        [Property]
        public bool Addition_is_commutative(Quality quality1, Quality quality2)
            => quality1 + quality2 == quality2 + quality1;

        [Property]
        public bool Addition_is_associative(
            Quality quality1,
            Quality quality2,
            Quality quality3
        )
        {
            var left = quality1 + (quality2 + quality3);
            var right = (quality1 + quality2) + quality3;

            return left == right;
        }

        [Property]
        public Property Subtraction_truncates_high(
            PositiveInt input1,
            PositiveInt input2
        )
        {
            var (raw1, raw2) = ((int)input1, (int)input2);
            var raw3 = raw1 - raw2;

            Quality quality1 = new(raw1), quality2 = new(raw2);
            var quality3 = quality1 - quality2;

            return (raw3 < raw1 || quality3 == Quality.MinValue)
                .Label($"raw difference: {raw3}, quality difference: {quality3}");
        }

        [Property]
        public Property Subtractive_identity_holds(Quality quality)
            =>  (quality - Quality.MinValue == quality)
                .Label($"0 on the right ({quality})")
                .And(Quality.MinValue - quality == Quality.MinValue)
                .Label($"0 on the left ({quality})");
    }
}
