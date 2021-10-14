namespace GildedRose.Test

open FsCheck
open GildedRose

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only legendary items should be generated.
[<NoComparison>]
type LegendaryItem = LegendaryItem of Item

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only backstage passes should be generated.
[<NoComparison>]
type BackstagePassItem = BackstagePassItem of Item

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only appreciating items should be generated.
[<NoComparison>]
type AppreciatingItem = AppreciatingItem of Item

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only depreciating items should be generated.
[<NoComparison>]
type DepreciatingItem = DepreciatingItem of Item


/// Contains logic for extending FsCheck's data-generation capabilities
/// with various GlidedRose-specific types.
type ProgramGenerators private() =
  static let shrinkItem (item : Item) =
    (item.Quality, item.SellIn)
    |> Arb.shrink
    |> Seq.map (fun (quality, sellIn) ->
        Item(Name=item.Name, Quality=quality, SellIn=sellIn)
    )

  static let generateKind ctor known =
    Arb.generate<Item>
    |> Gen.where (fun item -> item.Name = known)
    |> Gen.map ctor

  /// Generates a new item randomly, choosing from the various item kinds but
  /// with slight preference for depreciating or appreciating over the others.
  static member Item =
    let generate =
      gen {
        let! name = Gen.frequency [
          (4, Gen.constant KnownItems.AgedBrie)
          (4, Gen.constant KnownItems.Dex5Vest)
          (4, Gen.constant KnownItems.ManaCake)
          (4, Gen.constant KnownItems.Mongoose)
          (2, Gen.constant KnownItems.StageTix)
          (1, Gen.constant KnownItems.Sulfuras)
        ]

        let! quality =
          if name = KnownItems.Sulfuras
            then Gen.constant 80
            else Gen.choose (0, 50)

        let! sellIn =
          if name = KnownItems.Sulfuras
            then Gen.constant 0
            else Arb.generate<int>

        return Item(Name=name, Quality=quality, SellIn=sellIn)
      }

    Arb.fromGenShrink (generate, shrinkItem)

  /// Generates a new legendary item.
  static member LegendaryItem =
    Item(Name=KnownItems.Sulfuras, Quality=80, SellIn=0)
    |> LegendaryItem
    |> Gen.constant
    |> Arb.fromGen

  /// Generates a new backstage pass with random quality and sellIn values.
  static member BackstagePassItem =
    let generate =
      generateKind BackstagePassItem KnownItems.StageTix

    let shrink (BackstagePassItem item) =
      item
      |> shrinkItem
      |> Seq.map BackstagePassItem

    Arb.fromGenShrink (generate, shrink)

  /// Generates a new appreciating item with random quality and sellIn values.
  static member AppreciatingItem =
    let generate =
      generateKind AppreciatingItem KnownItems.AgedBrie

    let shrink (AppreciatingItem item) =
      item
      |> shrinkItem
      |> Seq.map AppreciatingItem

    Arb.fromGenShrink (generate, shrink)

  /// Generates a new depreciating item with random quality and sellIn values.
  static member DepreciatingItem =
    let generate =
      generateKind DepreciatingItem KnownItems.Mongoose

    let shrink (DepreciatingItem item) =
      item
      |> shrinkItem
      |> Seq.map DepreciatingItem

    Arb.fromGenShrink (generate, shrink)
