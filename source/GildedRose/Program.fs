namespace GildedRose

open GildedRose.Inventory

/// Since an item's "kind" (and thus, its behavior) might sometimes be
/// determined by its name, it's useful to have some well-known names.
[<RequireQualifiedAccess>]
module KnownItems =
  // Depreciating items
  let [<Literal>] Dex5Vest = "+5 Dexterity Vest"
  let [<Literal>] Mongoose = "Elixir of the Mongoose"

  // Conjured items
  let [<Literal>] ManaCake = "Conjured Mana Cake"

  // Appreciating items
  let [<Literal>] AgedBrie = "Aged Brie"

  // Backstage Passes
  let [<Literal>] StageTix = "Backstage passes to a TAFKAL80ETC concert"

  // Legendary items
  let [<Literal>] Sulfuras = "Sulfuras, Hand of Ragnaros"


module Program =
  [<EntryPoint>]
  let main _ =
    printfn "OMGHAI!"

    let items =
      [
        Depreciating  (KnownItems.Dex5Vest, Quality.Of 20uy, 10<days>)
        Appreciating  (KnownItems.AgedBrie, Quality.Of  0uy,  2<days>)
        Depreciating  (KnownItems.Mongoose, Quality.Of  7uy,  5<days>)
        Legendary     (KnownItems.Sulfuras, MagicQuality())
        BackstagePass (KnownItems.StageTix, Quality.Of 20uy, 15<days>)
        Conjured      (KnownItems.ManaCake, Quality.Of  6uy,  3<days>)
      ]

    for item in items do
      let name, quality, sellIn =
        match updateItem item with
        | Legendary (name, quality) ->
            // normalize for compatibility with existing approval test
            (name, byte quality, 0)

        // ⮝⮝⮝ legendary / ordinary ⮟⮟⮟

        | BackstagePass (name, quality, sellIn)
        | Appreciating  (name, quality, sellIn)
        | Depreciating  (name, quality, sellIn)
        | Conjured      (name, quality, sellIn) ->
            // normalize for compatibility with existing approval test
            (name, byte quality, int sellIn)

      printfn $"Item {{ Name = {name}, Quality = {quality}, SellIn = {sellIn} }}"

    printfn "Press <RETURN> to exit."
    System.Console.ReadLine() |> ignore
    0 // OK!
