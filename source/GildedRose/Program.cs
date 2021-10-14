using System.Collections.Generic;

namespace GildedRose
{
    using static System.Console;

    public class Program
    {
        // ReSharper disable once InconsistentNaming
        IList<Item>? Items;

        public static void Main()
        {
            WriteLine("OMGHAI!");

            var app = new Program
            {
                Items = new List<Item>
                {
                    new (){ Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20 },
                    new (){ Name = "Aged Brie", SellIn = 2, Quality = 0 },
                    new (){ Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7 },
                    new (){ Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 },
                    new ()
                        {
                            Name = "Backstage passes to a TAFKAL80ETC concert",
                            SellIn = 15,
                            Quality = 20
                        },
                    new (){ Name = "Conjured Mana Cake", SellIn = 3, Quality = 6 }
                }
           };

            app.UpdateQuality(app.Items);

            foreach (var item in app.Items)
            {
                WriteLine($"Item {{ Name = {item.Name}" +
                          $", Quality = {item.Quality}" +
                          $", SellIn = {item.SellIn} }}");
            }

            WriteLine("Press <RETURN> to exit.");
            ReadLine();
        }

        public void UpdateQuality(IList<Item>? items)
        {
            if (items is null) return;

            foreach (var t in items)
            {
                if (t.Name != "Aged Brie" && t.Name != "Backstage passes to a TAFKAL80ETC concert")
                {
                    if (t.Quality > 0)
                    {
                        if (t.Name != "Sulfuras, Hand of Ragnaros")
                        {
                            t.Quality -= 1;
                        }
                    }
                }
                else
                {
                    if (t.Quality < 50)
                    {
                        t.Quality += 1;

                        if (t.Name == "Backstage passes to a TAFKAL80ETC concert")
                        {
                            if (t.SellIn < 11)
                            {
                                if (t.Quality < 50)
                                {
                                    t.Quality += 1;
                                }
                            }

                            if (t.SellIn < 6)
                            {
                                if (t.Quality < 50)
                                {
                                    t.Quality += 1;
                                }
                            }
                        }
                    }
                }

                if (t.Name != "Sulfuras, Hand of Ragnaros")
                {
                    t.SellIn -= 1;
                }

                if (t.SellIn < 0)
                {
                    if (t.Name != "Aged Brie")
                    {
                        if (t.Name != "Backstage passes to a TAFKAL80ETC concert")
                        {
                            if (t.Quality > 0)
                            {
                                if (t.Name != "Sulfuras, Hand of Ragnaros")
                                {
                                    t.Quality -= 1;
                                }
                            }
                        }
                        else
                        {
                            t.Quality -= t.Quality;
                        }
                    }
                    else
                    {
                        if (t.Quality < 50)
                        {
                            t.Quality += 1;
                        }
                    }
                }
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
