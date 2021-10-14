using System;
using System.IO;
using System.Text;
using FsCheck;
using FsCheck.Xunit;

namespace GildedRose.Test
{
    using static Environment;

    public sealed class ProgramTests
    {
        [Property(MaxTest = 1)]
        public Property Program_output_meets_baseline_approval()
        {
            // arrange
            StringBuilder buffer = new();
            using StringWriter writer = new(buffer);
            using StringReader reader = new(NewLine);
            Console.SetOut(writer);
            Console.SetIn(reader);

            // act
            Program.Main();

            // assert
            var expected = @"OMGHAI!
Item { Name = +5 Dexterity Vest, Quality = 19, SellIn = 9 }
Item { Name = Aged Brie, Quality = 1, SellIn = 1 }
Item { Name = Elixir of the Mongoose, Quality = 6, SellIn = 4 }
Item { Name = Sulfuras, Hand of Ragnaros, Quality = 80, SellIn = 0 }
Item { Name = Backstage passes to a TAFKAL80ETC concert, Quality = 21, SellIn = 14 }
Item { Name = Conjured Mana Cake, Quality = 4, SellIn = 2 }
Press <RETURN> to exit.
";
            var actual = buffer.ToString();
            return string.Equals(expected, actual, StringComparison.Ordinal)
                .Label($"{NewLine}{nameof(expected)}: {expected}" +
                       $"{NewLine}{nameof(actual)}: {actual}");
        }
    }
}
