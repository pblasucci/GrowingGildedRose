using System;

namespace GildedRose;

/// Tracks the name of any inventory.
public interface IStock
{
    string Name { get; }

    IStock Update(Advance age, RateOfChange ramp);
}

/// An item with a constant value and no "shelf life".
public sealed record Legendary
    (string Name, MagicQuality Quality = default) : IStock
{
    /// <inheritdoc />
    public IStock Update(Advance _, RateOfChange __) => this;
}

/// Tracks any inventory which has both a value and a "shelf life".
public interface IOrdinary : IStock
{
    /// The value of a piece of inventory.
    Quality Quality { get; }

    /// The "shelf life" of a piece of inventory;
    /// when negative may impact the items quality.
    int SellIn { get; }
}

/// An item whose value decreases as its "shelf life" decreases.
public sealed record Depreciating
    (string Name, Quality Quality, int SellIn) : IOrdinary
{
    /// <inheritdoc />
    public IStock Update(Advance age, RateOfChange ramp)
    {
        var aged = age(SellIn);
        return this with { Quality = Quality - ramp(aged), SellIn = aged };
    }
}

/// An item whose value increases as its "shelf life" decreases.
public sealed record Appreciating
    (string Name, Quality Quality, int SellIn) : IOrdinary
{
    /// <inheritdoc />
    public IStock Update(Advance age, RateOfChange ramp)
    {
        var aged = age(SellIn);
        return this with { Quality = Quality + ramp(aged), SellIn = aged };
    }
}

/// An item whose value is subject to complex, "shelf life"-dependent rules.
public sealed record BackstagePass
    (string Name, Quality Quality, int SellIn) : IOrdinary
{
    /// <inheritdoc />
    public IStock Update(Advance age, RateOfChange ramp)
    {
        var (quality, sellIn) = age(SellIn) switch
        {
            var aged and < 0 => (Quality.MinValue, aged),

            //  NOTE
            //  ----
            //  Pass quality has a "hard cliff", based on "shelf life".
            //  However, until then, its value is calculated against
            //  the _current_ expiry (i.e. before advancing the clock).

            var aged when SellIn <= 5 => (Quality + 3, aged),
            var aged when SellIn <= 10 => (Quality + 2, aged),
            var aged => (Quality + 1, aged)
        };
        return this with { Quality = quality, SellIn = sellIn };
    }
}

/// Similar to a "Depreciating" item, but deteriorates twice as quickly.
public sealed record Conjured
    (string Name, Quality Quality, int SellIn) : IOrdinary
{
    /// <inheritdoc />
    public IStock Update(Advance age, RateOfChange ramp)
    {
        var aged = age(SellIn);
        return this with { Quality = Quality - 2 * ramp(aged), SellIn = aged };
    }
}

public delegate int Advance(int sellIn);

public delegate int RateOfChange(int sellIn);

public static class Inventory
{
    public static void Deconstruct(
        this IStock stock,
        out string name,
        out int quality,
        out int? sellIn
    )
    {
        switch (stock)
        {
            case Legendary legendary:
                name = stock.Name;
                quality = (int)legendary.Quality;
                sellIn = default;
                break;

            case IOrdinary ordinary:
                name = stock.Name;
                quality = (int)ordinary.Quality;
                sellIn = ordinary.SellIn;
                break;

            default:
                name = string.Empty;
                quality = default;
                sellIn = default;
                break;
        }
    }
}
