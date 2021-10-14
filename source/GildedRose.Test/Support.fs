namespace GildedRose.Test

// Since an item's "kind" (and thus, its behavior) might sometimes be
// determined by its name, it's useful to have some well-known names.
[<RequireQualifiedAccess>]
module KnownItems =
  // Depreciating items
  let [<Literal>] Dex5Vest = "+5 Dexterity Vest"
  let [<Literal>] Mongoose = "Elixir of the Mongoose"
  let [<Literal>] ManaCake = "Conjured Mana Cake"

  // Appreciating items
  let [<Literal>] AgedBrie = "Aged Brie"

  // Backstage Passes
  let [<Literal>] StageTix = "Backstage passes to a TAFKAL80ETC concert"

  // Legendary items
  let [<Literal>] Sulfuras = "Sulfuras, Hand of Ragnaros"
