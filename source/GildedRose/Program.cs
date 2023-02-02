using System.Collections.Generic;

namespace GildedRose;

using static System.Console;
using static KnownItems;

public static class Program
{
    public static void Main()
    {
        WriteLine("OMGHAI!");

        var items = new List<IStock>
        {
            new Depreciating(Dex5Vest, new Quality(20), SellIn: 10),
            new Appreciating(AgedBrie, new Quality(0), SellIn: 2),
            new Depreciating(Mongoose, new Quality(7), SellIn: 5),
            new Legendary(Sulfuras),
            new BackstagePass(StageTix, new Quality(20), SellIn: 15),
            new Conjured(ManaCake, new Quality(6), SellIn: 3)
        };

        foreach (var item in items)
        {
            // Update program state
            var (name, quality, sellIn) = item.Update(Advance, RateOfChange);

            // Display updated inventory
            var data = $"Name = {name}, Quality = {quality}, SellIn = {sellIn}";
            WriteLine($"Item {{ {data} }}");
        }

        WriteLine("Press <RETURN> to exit.");
        ReadLine();

        int Advance(int sellIn) => sellIn - 1;
        int RateOfChange(int sellIn) => sellIn < 0 ? 2 : 1;
    }
}
