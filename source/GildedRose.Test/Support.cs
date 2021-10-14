using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;

namespace GildedRose.Test
{
    using static Inventory;

    public static class Support
    {
        public static IInventoryItem AdvanceBy
        (
            this IInventoryItem item,
            PositiveInt totalDays
        ) =>
            Enumerable
                .Range(1, (int)totalDays)
                .Aggregate(item, (it, _) => UpdateItem(it));

        public static bool WithinInclusive<T>(this T value, T lower, T upper)
        {
            if (0 < Comparer<T>.Default.Compare(lower, upper))
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(upper),
                    "upper cannot be less than lower"
                );
            }

            return 0 <= Comparer<T>.Default.Compare(value, lower)
                && 0 <= Comparer<T>.Default.Compare(upper, value);
        }
    }

    public static class Arbitrary
    {
        public static Arbitrary<MagicQuality> MagicQuality() =>
            Arb.From(Gen.Constant(default(MagicQuality)));

        public static Arbitrary<IOrdinary> IOrdinary()
        {
            return Arb.From
            (
                Gen.OneOf<IOrdinary>
                (
                    GenerateOrdinary<Depreciating>(),
                    GenerateOrdinary<Conjured>(),
                    GenerateOrdinary<Appreciating>(),
                    GenerateOrdinary<BackstagePass>()
                )
            );

            static Gen<IOrdinary> GenerateOrdinary<T>() where T : IOrdinary =>
                from it in Arb.Generate<T>() select (IOrdinary) it;
        }
    }
}
