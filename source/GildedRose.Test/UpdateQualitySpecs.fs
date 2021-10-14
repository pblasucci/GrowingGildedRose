namespace GildedRose.Test

(* test infrastructure *)
open FsCheck
open FsCheck.Xunit

open type System.Environment

(* system under test *)
open GildedRose

/// Evaluates whether or not Program.UpdateQuality()
/// upholds certain invariants (based on given requirements).
[<Properties(Arbitrary=[|typeof<ProgramGenerators>|])>]
module UpdateQualitySpecs =

  let summarizeItemPair (item, item') =
    let dump prefix (it : Item) =
      $"{prefix}: {it.Name}, {it.SellIn} days, {it.Quality}"

    $"{dump (nameof item) item}{NewLine}{dump (nameof item') item'}"

  [<Property>]
  let ``after +N days, Legendary item is unchanged``
    (LegendaryItem item)
    (PositiveInt totalDays)
    =
    let items =
      [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
    //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
    let program = Program()
    for _ in 1 .. totalDays do
      program.UpdateQuality(items)

    match Array.tryHead items with
    | None -> false |@ "Items collection should NOT be empty!"
    | Some item' ->
        (item'.Name = item.Name &&
         item'.Quality = item.Quality &&
         item'.SellIn = item.SellIn)
        |@ summarizeItemPair (item, item')

  [<Property>]
  let ``after +N days, ordinary item has sellIn decreased by N``
    (item : Item)
    (PositiveInt totalDays)
    =
    (item.Name <> KnownItems.Sulfuras)
      ==> lazy (
        let items =
          [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
        //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
        let program = Program()
        for _ in 1 .. totalDays do
          program.UpdateQuality(items)

        match Array.tryHead items with
        | None -> false |@ "Items collection should NOT be empty!"
        | Some item' ->
            item.SellIn - item'.SellIn = totalDays
            |@ summarizeItemPair (item, item')
      )

  [<Property>]
  let ``after +N days, ordinary item has 0 <= quality <= 50``
    (item : Item)
    (PositiveInt totalDays)
    =
    (item.Name <> KnownItems.Sulfuras)
      ==> lazy (
        let items =
          [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
        //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
        let program = Program()
        for _ in 1 .. totalDays do
          program.UpdateQuality(items)

        match Array.tryHead items with
        | None -> false |@ "Items collection should NOT be empty!"
        | Some item' ->
            (0 <= item.Quality && item.Quality <= 50)
            |@ summarizeItemPair (item, item')
      )

  [<Property>]
  let ``after +N days, depreciating item has lesser quality``
    (DepreciatingItem item)
    (PositiveInt totalDays)
    =
    let items =
      [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
    //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
    let program = Program()
    for _ in 1 .. totalDays do
      program.UpdateQuality(items)

    match Array.tryHead items with
    | None -> false |@ "Items collection should NOT be empty!"
    | Some item' ->
        (item'.Quality < item.Quality || item'.Quality = 0)
        |@ summarizeItemPair (item, item')

  [<Property>]
  let ``after +1 days, depreciating item has 0 <= abs(quality change) <= 2``
    (DepreciatingItem item)
    =
    let items =
      [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
    //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
    let program = Program()
    program.UpdateQuality(items)

    match Array.tryHead items with
    | None -> false |@ "Items collection should NOT be empty!"
    | Some item' ->
        let delta = abs (item.Quality - item'.Quality)
        [0 .. 2]
        |> List.exists ((=) delta)
        |@ summarizeItemPair (item, item')

  [<Property>]
  let ``after +N days, appreciating item has greater quality``
    (AppreciatingItem item)
    (PositiveInt totalDays)
    =
    let items =
      [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
    //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
    let program = Program()
    for _ in 1 .. totalDays do
      program.UpdateQuality(items)

    match Array.tryHead items with
    | None -> false |@ "Items collection should NOT be empty!"
    | Some item' ->
        (item.Quality < item'.Quality || item'.Quality = 50)
        |@ summarizeItemPair (item, item')

  [<Property>]
  let ``after +1 days, appreciating item has 0 <= abs(quality change) <= 2``
    (AppreciatingItem item)
    =
    let items =
      [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
    //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
    let program = Program()
    program.UpdateQuality(items)

    match Array.tryHead items with
    | None -> false |@ "Items collection should NOT be empty!"
    | Some item' ->
        let delta = abs (item'.Quality - item.Quality)
        [0 .. 2]
        |> List.exists ((=) delta)
        |@ summarizeItemPair (item, item')

  [<Property>]
  let ``after +1 days, backstage pass has no quality if sellIn is negative``
    (PositiveInt quality)
    =
    (0 < quality && quality < 50)
      ==> lazy (
        let item = Item(Name=KnownItems.StageTix, Quality=quality, SellIn=0)

        let items =
          [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
        //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
        let program = Program()
        program.UpdateQuality(items)

        match Array.tryHead items with
        | None -> false |@ "Items collection should NOT be empty!"
        | Some item' -> item'.Quality = 0 |@ summarizeItemPair (item, item')
      )

  [<Property>]
  let ``after +1 days, backstage pass has quality reduced by appropriately``
    (BackstagePassItem item)
    =
    let items =
      [| Item(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn) |]
    //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
    let program = Program()
    program.UpdateQuality(items)

    match Array.tryHead items with
    | None -> false |@ "Items collection should NOT be empty!"

    | Some item' when item'.SellIn < 0 ->
        item'.Quality = 0 |@ summarizeItemPair (item, item')

    //  NOTE
    //  ----
    //  Pass quality has a "hard cliff", based on "shelf life".
    //  However, until then, its value is calculated against
    //  the _current_ expiry (i.e. before advancing the clock).

    | Some item' when item.SellIn <= 5 ->
        (item'.Quality - item.Quality = 3 || item'.Quality = 50)
        |@ summarizeItemPair (item, item')

    | Some item' when item.SellIn <= 10 ->
        (item'.Quality - item.Quality = 2 || item'.Quality = 50)
        |@ summarizeItemPair (item, item')

    | Some item' ->
        (item'.Quality - item.Quality = 1 || item'.Quality = 50)
        |@ summarizeItemPair (item, item')
