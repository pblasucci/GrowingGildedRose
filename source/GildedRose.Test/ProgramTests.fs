namespace GildedRose.Test

(* test infrastructure *)
open System
open System.IO
open System.Text
open FsCheck
open FsCheck.Xunit

open type Environment

(* system under test *)
open GildedRose

type ProgramTests() =
  [<Property(MaxTest = 1)>]
  member _.``Program output meets baseline approval`` () =
    // arrange
    let buffer = StringBuilder()
    use writer = new StringWriter(buffer)
    use reader = new StringReader(NewLine)
    Console.SetOut(writer)
    Console.SetIn(reader)

    // act
    Array.empty |> Program.main |> ignore

    // assert
    let expected = "OMGHAI!
Item { Name = +5 Dexterity Vest, Quality = 19, SellIn = 9 }
Item { Name = Aged Brie, Quality = 1, SellIn = 1 }
Item { Name = Elixir of the Mongoose, Quality = 6, SellIn = 4 }
Item { Name = Sulfuras, Hand of Ragnaros, Quality = 80, SellIn = 0 }
Item { Name = Backstage passes to a TAFKAL80ETC concert, Quality = 21, SellIn = 14 }
Item { Name = Conjured Mana Cake, Quality = 4, SellIn = 2 }
Press <RETURN> to exit.
"
    let actual = string buffer
    expected = actual |@ $"{NewLine}{expected}{NewLine}{actual}"
