namespace GildedRose.Test

open FsCheck
open GildedRose
open GildedRose.Inventory

type OldItem = GildedRose.Item
type NewItem = GildedRose.Inventory.Item


/// Utilities for converting into or from types related to:
/// the application, the domain, or the test suite.
[<AutoOpen>]
module Conversions =
  /// Produces a constructor for a non-legendary item,
  /// using an existing Inventory.Item as a "template"
  /// (n.b.) if given a legendary item, no constructor is returned.
  let (|MakeOrdinary|_|) (item : Item) =
    match item with
    | BackstagePass _ -> Some (fun (n, q, s) -> BackstagePass (n, q, s))
    | Appreciating  _ -> Some (fun (n, q, s) -> Appreciating  (n, q, s))
    | Depreciating  _ -> Some (fun (n, q, s) -> Depreciating  (n, q, s))
    | Legendary     _ -> None

  /// Extracts the name from an Inventory.Item.
  let (|Name|) (item : Item) =
    match item with
    | BackstagePass (name=name)
    | Appreciating  (name=name)
    | Depreciating  (name=name)
    | Legendary     (name=name) -> name

  /// Extracts the quality from an Inventory.Item.
  let (|Quality|) (item : Item) =
    match item with
    | BackstagePass (quality=quality)
    | Appreciating  (quality=quality)
    | Depreciating  (quality=quality) -> int (byte quality)
    | Legendary     (quality=quality) -> int (byte quality)

  /// Extracts the expiration date from an Inventory.Item
  /// (n.b. will not return a value for legendary items).
  let (|SellIn|_|) (item : Item) =
    match item with
    | BackstagePass (sellIn=sellIn)
    | Appreciating  (sellIn=sellIn)
    | Depreciating  (sellIn=sellIn) -> Some (int sellIn)
    | Legendary     _               -> None

  /// Decomposes a GildedRose.Item instance
  /// into a three-tuple of it constituent fields.
  let inline (|OldItem|) (item : OldItem) =
    OldItem(item.Name, item.Quality, item.SellIn)

  /// Extracts the name of the variant from a Inventory.Item
  let (|ItemKind|) (item : Item) =
    match item with
    | BackstagePass _ -> nameof BackstagePass
    | Appreciating  _ -> nameof Appreciating
    | Depreciating  _ -> nameof Depreciating
    | Legendary     _ -> nameof Legendary


/// Defines an 8-bit whole number value in the range 1 to 255, inclusive,
type NonZeroByte = NonZeroByte of byte

/// Defines generators for common (i.e. non-Inventory) test inputs.
type SimpleGenerators =
  /// Generates (and shrinks) a randomly-valued NonZeroByte.
  static member NonZeroByte =
    let generate =
      Arb.generate<byte>
      |> Gen.where (fun v -> 0uy < v)
      |> Gen.map NonZeroByte

    let shrink (NonZeroByte value) =
      value
      |> Arb.shrink
      |> Seq.where (fun v -> 0uy < v)
      |> Seq.map NonZeroByte

    Arb.fromGenShrink (generate, shrink)
