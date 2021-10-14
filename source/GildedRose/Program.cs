using System;
using System.Collections.Generic;
using GildedRose.Interop;

using static System.Console;
using static GildedRose.Inventory;

using StockItem = GildedRose.Inventory.Item;

namespace GildedRose
{
    public class Program
    {
        // ReSharper disable once InconsistentNaming
        private IList<Item>? Items;

        public static void Main()
        {
            WriteLine("OMGHAI!");

            var app = new Program
            {
                Items = new List<Item>
                {
                    new(){Name = KnownItems.Dex5Vest, SellIn = 10, Quality = 20},
                    new(){Name = KnownItems.AgedBrie, SellIn =  2, Quality =  0},
                    new(){Name = KnownItems.Mongoose, SellIn =  5, Quality =  7},
                    new(){Name = KnownItems.Sulfuras, SellIn =  0, Quality = 80},
                    new(){Name = KnownItems.StageTix, SellIn = 15, Quality = 20},
                    new(){Name = KnownItems.ManaCake, SellIn =  3, Quality =  6}
                }
            };

            UpdateItems(app.Items);

            foreach (var item in app.Items)
            {
                WriteLine($"Item {{ Name = {item.Name}" +
                          $", Quality = {item.Quality}" +
                          $", SellIn = {item.SellIn} }}");
            }

            WriteLine("Press <RETURN> to exit.");
            ReadLine();
        }

        public static void UpdateItems(IList<Item>? items)
        {
            if (items is null) return;

            foreach (var item in items)
            {
                var evolved = Evolve(item);
                var (_, quality, sellIn) = UpdateItem(evolved);
                item.Quality = quality;
                item.SellIn = sellIn;
            }

            static StockItem Evolve(Item item)
            {
                var quality = Quality.Of((byte) item.Quality);

                return item.Name switch
                {
                    KnownItems.Sulfuras and var name =>
                        StockItem.NewLegendary(name, default),

                    KnownItems.StageTix and var name =>
                        StockItem.NewBackstagePass(name, quality, item.SellIn),

                    KnownItems.AgedBrie and var name =>
                        StockItem.NewAppreciating(name, quality, item.SellIn),

                    KnownItems.ManaCake and var name =>
                        StockItem.NewConjured(name, quality, item.SellIn),

                    /* depreciating */ var name =>
                        StockItem.NewDepreciating(name, quality, item.SellIn)
                };
            }
        }
    }

    public class Item
    {
        public string Name { get; set; } = "";

        public int SellIn { get; set; }

        public int Quality { get; set; }
    }
}
