namespace GildedRose.Test

(* test infrastructure *)
open FsCheck
open FsCheck.Xunit

open type System.Environment

(* system under test *)
open GildedRose
open GildedRose.Inventory

/// Evaluates whether or not Inventory.updateItem()
/// upholds certain invariants (based on given requirements).
[<Properties(Arbitrary=[|
  typeof<SimpleGenerators>
  typeof<InventoryGenerators>
|])>]
module UpdateItemSpecs =
  let advanceBy (PositiveInt totalDays) item =
    [1 .. totalDays] |> List.fold (fun it _ -> updateItem it) item

  [<Property>]
  let ``after +N days, Legendary item is unchanged``
    (OnlyLegendary item)
    totalDays
    =
    let item' = item |> advanceBy totalDays

    match (item, item') with
    | Legendary (name=name ),
      Legendary (name=name') ->
        (name = name' |@ $"{nameof name}: {name} ≠ {name'}")

    | ItemKind case, ItemKind case' ->
        false |@ $"Unexpected variant: {case} is not {case'}"

  [<Property>]
  let ``after +N days, ordinary item has sellIn decreased by N``
    (OnlyOrdinary item)
    (PositiveInt days as totalDays)
    =
    let item' = item |> advanceBy totalDays

    match (item, item') with
    | SellIn sellIn, SellIn sellIn' ->
        sellIn - sellIn' = days
        |@ $"{nameof (|SellIn|_|)}: {sellIn} - {sellIn'} ≠ {days}"

    | ItemKind case, ItemKind case' ->
        false |@ $"Unexpected variant: {case} is not {case'}"

  [<Property>]
  let ``after +N days, depreciating item has lesser quality``
    (OnlyDepreciating item)
    totalDays
    =
    let item' = item |> advanceBy totalDays

    match (item, item') with
    | Depreciating (quality=quality ),
      Depreciating (quality=quality') ->
        (quality' < quality || quality' = Quality.MinValue)
        |@ $"{nameof quality}: {quality'} ≮ {quality} ∧ ¬{nameof Quality.MinValue}"

    | ItemKind case, ItemKind case' ->
        false |@ $"Unexpected variant: {case} is not {case'}"

  [<Property>]
  let ``after +1 days, depreciating item has 0 <= abs(quality change) <= 2``
    (OnlyDepreciating item)
    =
    let item' = item |> advanceBy (PositiveInt 1)

    match (item, item') with
    | Depreciating (quality=quality ),
      Depreciating (quality=quality') ->
        let delta = (quality - quality') |> byte |> int |> abs
        (0 <= delta && delta <= 2)
        |@ $"{nameof quality}: |{quality} - {quality'}| ∉ {{0, 1, 2}}"

    | ItemKind case, ItemKind case' ->
        false |@ $"Unexpected variant: {case} is not {case'}"

  [<Property>]
  let ``after +N days, appreciating item has greater quality``
    (OnlyAppreciating item)
    totalDays
    =
    let item' = item |> advanceBy totalDays
    match (item, item') with
    | Appreciating (quality=quality ),
      Appreciating (quality=quality') ->
        (quality < quality' || quality' = Quality.MaxValue)
        |@ $"{nameof quality}: {quality} ≮ {quality'} ∧ ¬{nameof Quality.MaxValue}"

    | ItemKind case, ItemKind case' ->
        false |@ $"Unexpected variant: {case} is not {case'}"

  [<Property>]
  let ``after +1 days, appreciating item has 0 <= abs(quality change) <= 2``
    (OnlyAppreciating item)
    =
    let item' = item |> advanceBy (PositiveInt 1)
    match (item, item') with
    | Appreciating (quality=quality ),
      Appreciating (quality=quality') ->
        let delta = (quality' - quality) |> byte |> int |> abs
        (0 <= delta && delta <= 2)
        |@ $"{nameof quality}: |{quality'} - {quality}| ∉ {{0, 1, 2}}"

    | ItemKind case, ItemKind case' ->
        false |@ $"Unexpected variant: {case} is not {case'}"

  [<Property>]
  let ``after +1 days, backstage pass has no quality if sellIn is negative``
    (NonZeroByte worth)
    =
    let item = BackstagePass(KnownItems.StageTix, Quality.Of worth, 0<days>)

    match item |> advanceBy (PositiveInt 1) with
    | BackstagePass (_, quality', sellIn') ->
        quality' = Quality.MinValue
        |@ $"{sellIn'} days < 0, but {quality'} ≠ {nameof Quality.MinValue}"

    | ItemKind case' ->
        let (ItemKind case) = item
        false |@ $"Unexpected variant: {case}, {case'}"

  [<Property>]
  let ``after +1 days, backstage pass has quality reduced by appropriately``
    (OnlyBackstagePass item)
    =
    let item' = item |> advanceBy (PositiveInt 1)

    match (item, item') with
    | BackstagePass (quality=quality),
      BackstagePass (quality=quality'; sellIn=sellIn')
      when sellIn' < 0<days> ->
        quality' = Quality.MinValue
        |@ $"{nameof quality}: was {quality}, is {quality'} (goal: 0)"

    //  NOTE
    //  ----
    //  Pass quality has a "hard cliff", based on "shelf life".
    //  However, until then, its value is calculated against
    //  the _current_ expiry (i.e. before advancing the clock).

    | BackstagePass (quality=quality; sellIn=sellIn),
      BackstagePass (quality=quality')
      when sellIn <= 5<days> ->
        (quality' - quality = Quality.Of 3uy || quality' = Quality.MaxValue)
        |@ $"{nameof quality}: was {quality}, is {quality'} (goal: 3)"

    | BackstagePass (quality=quality; sellIn=sellIn),
      BackstagePass (quality=quality')
      when sellIn <= 10<days> ->
        (quality' - quality = Quality.Of 2uy || quality' = Quality.MaxValue)
        |@ $"{nameof quality}: was {quality} is {quality'} (goal: 2)"

    | BackstagePass (quality=quality ),
      BackstagePass (quality=quality') ->
        (quality' - quality = Quality.Of 1uy || quality' = Quality.MaxValue)
        |@ $"{nameof quality}: was {quality}, is {quality'} (goal: 1)"

    | ItemKind case, ItemKind case' ->
        false |@ $"Unexpected variant: {case} is not {case'}"
