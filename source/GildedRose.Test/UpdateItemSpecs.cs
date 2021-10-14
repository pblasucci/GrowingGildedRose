using System;
using FsCheck;
using FsCheck.Xunit;

namespace GildedRose.Test
{
    using static Math;

    [Properties(Arbitrary = new[]{ typeof(Test.Arbitrary) })]
    public sealed class UpdateItemSpecs
    {
        [Property]
        public Property After_N_days_Legendary_item_is_unchanged
        (
            Legendary item,
            PositiveInt totalDays
        ) =>
            item.AdvanceBy(totalDays) switch
            {
                Legendary(var name, _) =>
                    string
                        .Equals(item.Name, name, StringComparison.Ordinal)
                        .Label($"{item.Name} ≠ {name}"),

                var badItem =>
                    false.Label($"Expected Legendary, but got: {badItem}.")
            };

        [Property]
        public Property After_N_days_ordinary_item_has_SellIn_decreased_by_N
        (
            IOrdinary item,
            PositiveInt totalDays
        ) =>
           item.AdvanceBy(totalDays) switch
           {
                IOrdinary {SellIn: var sellIn} =>
                    (item.SellIn - sellIn == (int) totalDays)
                        .Label($"{item.SellIn} - {sellIn} ≠ {(int) totalDays}"),

                var badItem =>
                    false.Label($"Expected IOrdinary, but got: {badItem}.")
           };

        [Property]
        public Property After_N_days_depreciating_item_has_lesser_quality
        (
            Depreciating item,
            PositiveInt totalDays
        ) =>
            item.AdvanceBy(totalDays) switch
            {
                Depreciating (_, var quality, _) =>
                    (quality < item.Quality || quality == Quality.MinValue)
                        .Label($"{quality} ≮ {item.Quality} ∧ ¬{Quality.MinValue}"),

                var badItem =>
                    false.Label($"Expected Depreciating, but got: {badItem}.")
            };

        [Property]
        public Property After_1_day_depreciating_has_quality_change_between_0_and_2
        (
            Depreciating item
        ) =>
            item.AdvanceBy(PositiveInt.NewPositiveInt(1)) switch
            {
                Depreciating (_, var quality, _) =>
                    Abs((int) (item.Quality - quality))
                        .WithinInclusive(lower: 0, upper: 2)
                        .Label($"|{item.Quality} - {quality}| ∉ {{0, 1, 2}}"),

                var badItem =>
                    false.Label($"Expected Depreciating, but got: {badItem}.")
            };

        [Property]
        public Property After_N_days_conjured_item_has_lesser_quality
        (
            Conjured item,
            PositiveInt totalDays
        ) =>
            item.AdvanceBy(totalDays) switch
            {
                Conjured (_, var quality, _) =>
                    (quality < item.Quality || quality == Quality.MinValue)
                    .Label($"{quality} ≮ {item.Quality} ∧ ¬{Quality.MinValue}"),

                var badItem =>
                    false.Label($"Expected Conjured, but got: {badItem}.")
            };

        [Property]
        public Property After_1_day_conjured_has_quality_change_between_0_and_4
        (
            Conjured item
        ) =>
            item.AdvanceBy(PositiveInt.NewPositiveInt(1)) switch
            {
                Conjured (_, var quality, _) =>
                    Abs((int) (item.Quality - quality))
                        .WithinInclusive(lower: 0, upper: 4)
                        .Label($"|{item.Quality} - {quality}| ∉ {{0, 1, 2, 3, 4}}"),

                var badItem =>
                    false.Label($"Expected Conjured, but got: {badItem}.")
            };

        [Property]
        public Property After_N_days_appreciating_item_has_greater_quality
        (
            Appreciating item,
            PositiveInt totalDays
        ) =>
            item.AdvanceBy(totalDays) switch
            {
                Appreciating (_, var quality, _) =>
                    (item.Quality < quality || quality == Quality.MaxValue)
                    .Label($"{item.Quality} ≮ {quality} ∧ ¬{Quality.MaxValue}"),

                var badItem =>
                    false.Label($"Expected Appreciating, but got: {badItem}.")
            };

        [Property]
        public Property After_1_day_appreciating_has_quality_change_between_0_and_2
        (
            Appreciating item
        ) =>
            item.AdvanceBy(PositiveInt.NewPositiveInt(1)) switch
            {
                Appreciating (_, var quality, _) =>
                    Abs((int) (item.Quality - quality))
                        .WithinInclusive(lower: 0, upper: 2)
                        .Label($"|{quality} - {item.Quality}| ∉ {{0, 1, 2}}"),

                var badItem =>
                    false.Label($"Expected Appreciating, but got: {badItem}.")
            };

        [Property]
        public Property After_1_day_backstage_pass_has_no_quality_if_SellIn_is_negative
        (
            PositiveInt worth
        )
        {
            var item = new BackstagePass
            (
                KnownItems.StageTix,
                new Quality((int) worth),
                0
            );

            return item.AdvanceBy(PositiveInt.NewPositiveInt(1)) switch
            {
                BackstagePass (_, var quality, var sellIn) =>
                    (quality == Quality.MinValue)
                        .Label($"{sellIn} < 0, but {quality} ≠ {Quality.MinValue}"),

                var badItem =>
                    false.Label($"Expected BackstagePass, but got: {badItem}.")
            };
        }

        [Property]
        public Property After_1_days_backstage_pass_has_quality_reduced_appropriately
        (
            BackstagePass item
        )
        {
            return item.AdvanceBy(PositiveInt.NewPositiveInt(1)) switch
            {
                BackstagePass (_, var quality, var sellIn) when sellIn < 0 =>
                    (quality == Quality.MinValue)
                    .Label($"{sellIn} < 0, but {Quality.MinValue} ≠ {quality}"),

                //  NOTE
                //  ----
                //  Pass quality has a "hard cliff", based on "shelf life".
                //  However, until then, its value is calculated against
                //  the _current_ expiry (i.e. before advancing the clock).

                BackstagePass(_, var quality, _) when item.SellIn <= 5 =>
                    ExpectedWorth(quality, item.Quality, 3),

                BackstagePass(_, var quality, _) when item.SellIn <= 10 =>
                    ExpectedWorth(quality, item.Quality, 2),

                BackstagePass(_, var quality, _) =>
                    ExpectedWorth(quality, item.Quality, 1),

                var badItem =>
                    false.Label($"Expected BasckstagePass, but got: {badItem}.")
            };

            static Property ExpectedWorth(Quality one, Quality two, int target)
            {
                var okay = one - two == new Quality(target)
                        || one == Quality.MaxValue;

                return okay.Label($"{one} - {two} ≠ {target} ∧ ¬{Quality.MaxValue}");
            }
        }
    }
}
