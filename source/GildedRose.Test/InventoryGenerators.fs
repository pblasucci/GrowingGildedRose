namespace GildedRose.Test

open FsCheck
open GildedRose.Inventory

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only legendary items should be generated.
type OnlyLegendary = OnlyLegendary of Item

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only backstage passes should be generated
type OnlyBackstagePass = OnlyBackstagePass of Item

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only appreciating items should be generated.
type OnlyAppreciating = OnlyAppreciating of Item

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only depreciating items should be generated.
type OnlyDepreciating = OnlyDepreciating of Item

/// Wrapper to simplify data generation (with FsCheck);
/// Signals that only non-legendary items should be generated.
type OnlyOrdinary = OnlyOrdinary of Item


/// Contains logic for extending FsCheck's data-generation capabilities
/// with various GlidedRose.Inventory-specific types.
type InventoryGenerators() =
  static let arbForOneCase (|Unwrap|) ctor' check (|Get|_|) ctor =
    let generate =
      Arb.generate<Item>
      |> Gen.where check
      |> Gen.map ctor'

    let shrink (Unwrap item) =
      match item with
      | Get (name, quality, sellIn) ->
          Seq.zip3 (Arb.shrink name) (Arb.shrink quality) (Arb.shrink sellIn)
          |> Seq.map (fun (n, q, s) -> ctor' (ctor (n, q, s)))
      | _ ->
          Seq.empty //NOTE this should never happen!

    Arb.fromGenShrink (generate, shrink)

  /// Generates a new instance of Quality with its value
  /// constrained to Quality.MinValue ..Quality.MaxValue, inclusive.
  static member Quality =
    let minQ, maxQ =
      (byte Quality.MinValue, byte Quality.MaxValue)

    let generate =
      Gen.choose (int minQ, int maxQ)
      |> Gen.map (byte >> Quality.Of)

    let shrink quality =
      quality
      |> byte
      |> Arb.shrink
      |> Seq.map Quality.Of

    Arb.fromGenShrink (generate, shrink)

  /// Generates a new item, choosing randonaly from the various item kinds but
  /// with slight preference for depreciating or appreciating over the others.
  static member Item =
    let generate =
      gen {
        let! (NonEmptyString name) = Arb.generate<_>
        let! ctor = Gen.frequency [
          (1, Gen.constant (fun _ _ -> Legendary(name, MagicQuality())))
          // ⮝⮝⮝ legendary / ordinary ⮟⮟⮟
          (2, Gen.constant (fun q s -> BackstagePass (name, q, s)))
          (4, Gen.constant (fun q s -> Appreciating  (name, q, s)))
          (4, Gen.constant (fun q s -> Depreciating  (name, q, s)))
        ]
        let! quality, sellIn = Arb.generate<_>
        return ctor quality sellIn
      }

    let shrink item =
        match item with
        | Legendary (name, quality) ->
            name |> Arb.shrink |> Seq.map (fun n -> Legendary (n, quality))
        // ⮝⮝⮝ legendary / ordinary ⮟⮟⮟
        | BackstagePass (name, quality, sellIn) & MakeOrdinary ctor
        | Appreciating  (name, quality, sellIn) & MakeOrdinary ctor
        | Depreciating  (name, quality, sellIn) & MakeOrdinary ctor ->
            Seq.zip3
              (Arb.shrink name) (Arb.shrink quality) (Arb.shrink sellIn)
            |> Seq.map ctor
        // ⮟⮟⮟ NOTE this should never happen ⮟⮟⮟
        | _ -> failwith $"Unexpected variant: {item}"

    Arb.fromGenShrink (generate, shrink)

  /// Generates a new legendary item.
  static member OnlyLegendary =
    let generate =
      Arb.generate<_>
      |> Gen.map (
          function
          | NonEmptyString name ->
              let item = Legendary (name, MagicQuality())
              OnlyLegendary (item)
      )

    let shrink (OnlyLegendary item) =
      match item with
      | Legendary (name, quality) ->
          name
          |> Arb.shrink
          |> Seq.map (fun n -> OnlyLegendary (Legendary (n, quality)))
      | _ -> Seq.empty //NOTE this should never happen!

    Arb.fromGenShrink (generate, shrink)

  /// Generates a new backstage pass with random quality and sellIn values.
  static member OnlyBackstagePass =
    arbForOneCase
      (function OnlyBackstagePass item -> item)
      OnlyBackstagePass
      (function BackstagePass _ -> true | _ -> false)
      (function BackstagePass (n, q, s) -> Some (n, q, s) | _ -> None)
      BackstagePass

  /// Generates a new appreciating item with random quality and sellIn values.
  static member OnlyAppreciating =
    arbForOneCase
      (function OnlyAppreciating item -> item)
      OnlyAppreciating
      (function Appreciating _ -> true | _ -> false)
      (function Appreciating (n, q, s) -> Some (n, q, s) | _ -> None)
      Appreciating

  /// Generates a new depreciating item with random quality and sellIn values.
  static member OnlyDepreciating =
    arbForOneCase
      (function OnlyDepreciating item -> item)
      OnlyDepreciating
      (function Depreciating _ -> true | _ -> false)
      (function Depreciating (n, q, s) -> Some (n, q, s) | _ -> None)
      Depreciating

  /// Generates a new non-legendary items with random quality and sellIn values.
  static member OnlyOrdinary =
    let generate =
      Gen.oneof [
        Arb.generate<OnlyBackstagePass>
        |> Gen.map (function OnlyBackstagePass it -> it)

        Arb.generate<OnlyAppreciating>
        |> Gen.map (function OnlyAppreciating it -> it)

        Arb.generate<OnlyDepreciating>
        |> Gen.map (function OnlyDepreciating it -> it)
      ]
      |> Gen.map OnlyOrdinary

    Arb.fromGen generate
