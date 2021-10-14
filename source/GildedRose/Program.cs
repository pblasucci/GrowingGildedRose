using System;
using System.Collections.Generic;

namespace GildedRose
{
    using static Console;
    using static Inventory;
    using static KnownItems;

    public static class Program
    {
        public static void Main()
        {
            WriteLine("OMGHAI!");

            var items = new List<IInventoryItem>
            {
                new Depreciating  (Dex5Vest, new Quality(20), SellIn: 10),
                new Appreciating  (AgedBrie, new Quality( 0), SellIn:  2),
                new Depreciating  (Mongoose, new Quality( 7), SellIn:  5),
                new Legendary     (Sulfuras),
                new BackstagePass (StageTix, new Quality(20), SellIn: 15),
                new Conjured      (ManaCake, new Quality( 6), SellIn:  3)
            };

            foreach (var item in items)
            {
                var (quality, sellIn) =
                    // Update program state
                    UpdateItem(item) switch
                    {
                        IOrdinary
                        {
                            Quality: var value,
                            SellIn: var days
                        } => ((int)value, days),

                        // if it's not ordinary, it must be legendary
                        _ => ((int)new MagicQuality(), 0)

                    };

                // Display updated inventory
                WriteLine($"Item {{ Name = {item.Name}" +
                          $", Quality = {quality}" +
                          $", SellIn = {sellIn} }}");
            }

            WriteLine("Press <RETURN> to exit.");
            ReadLine();
        }
    }
}
