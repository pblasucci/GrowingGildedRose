using System;

namespace GildedRose
{
    public static class Inventory
    {
        /// Change the quality and "shelf life" for an Item  (i.e. apply
        /// appropriate rules for the passage of a single "business day").
        public static IInventoryItem UpdateItem(IInventoryItem stock) =>
            stock switch
            {
                null => throw new ArgumentNullException(nameof(stock)),

                IOrdinary ordinary => UpdateOrdinary(ordinary),

                // if it's not ordinary, it must be legendary
                _ => stock // Legendary things never change!
            };

        private static IOrdinary UpdateOrdinary(IOrdinary ordinary)
        {
            var agedTo = ordinary.SellIn - 1; // days
            var rateOfChange = agedTo < 0 ? 2 : 1;

            return ordinary switch
            {
                Depreciating { Quality: var quality } item => item with
                {
                    SellIn = agedTo,
                    Quality = quality - new Quality(rateOfChange)
                },
                Appreciating { Quality: var quality } item => item with
                {
                    SellIn = agedTo,
                    Quality = quality + new Quality(rateOfChange)
                },
                Conjured { Quality: var quality } item => item with
                {
                    SellIn = agedTo,
                    Quality = quality - new Quality(2 * rateOfChange)
                },
                BackstagePass item when agedTo < 0 => item with
                {
                    SellIn = agedTo,
                    Quality = Quality.MinValue
                },
                //  NOTE
                //  ----
                //  Pass quality has a "hard cliff", based on "shelf life".
                //  However, until then, its value is calculated against
                //  the _current_ expiry (i.e. before advancing the clock).
                BackstagePass { Quality: var quality, SellIn: <= 5 } item => item with
                {
                    SellIn = agedTo,
                    Quality = quality + new Quality(3)
                },
                BackstagePass { Quality: var quality, SellIn: <= 10 } item => item with
                {
                    SellIn = agedTo,
                    Quality = quality + new Quality(2)
                },
                BackstagePass { Quality: var quality } item => item with
                {
                    SellIn = agedTo,
                    Quality = quality + new Quality(1)
                },

                _ => throw new InvalidProgramException($"Inventory unknown : {ordinary}")
            };
        }
    }
}
