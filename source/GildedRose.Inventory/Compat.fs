namespace GildedRose.Interop

open System.Runtime.CompilerServices
open GildedRose.Inventory

[<Extension>]
type ItemExtensions =
  /// Decomposes Inventory.Item into a triple (name, quality, and sellIn)
  /// whose values have been normalized for consumption from C#
  /// (n.b. for legendary items, sellIn is always zero).
  [<Extension>]
  static member Deconstruct
    (
      item : Item,
      name : outref<string>,
      quality : outref<byte>,
      sellIn : outref<int>
    )
    =
    match item with
    | Legendary (name', quality') ->
        name    <- name'
        quality <- byte quality'
        sellIn  <- 0

    | BackstagePass (name', quality', sellIn')
    | Appreciating  (name', quality', sellIn')
    | Conjured      (name', quality', sellIn')
    | Depreciating  (name', quality', sellIn') ->
        name    <- name'
        quality <- byte quality'
        sellIn  <- int sellIn'
