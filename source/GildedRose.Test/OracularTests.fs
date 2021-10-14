namespace GildedRose.Test

(* test infrastructure *)
open FsCheck
open FsCheck.Xunit

open type System.Environment

(* system under test *)
open GildedRose

[<Properties(Arbitrary=[| typeof<ProgramGenerators> |])>]
module OracularTests =
  [<Property>]
  let ``after +N days, UpdateQuality and UpdateItems produce the same results``
    (NonEmptyArray (items : OldItem array))
    (PositiveInt totalDays)
    =
    // arrange
    let oldItems =
      [|
        for item in items do
          OldItem(Name=item.Name, Quality=item.Quality, SellIn=item.SellIn)
      |]
    //NOTE ⮝⮝⮝ fully copy to preserve original -- because mutable
    for _ in 1 .. totalDays do Program.UpdateQuality(oldItems)
    let expected = oldItems |> Seq.sortBy (fun item -> item.Name)

    // act
    for _ in 1 .. totalDays do Program.UpdateItems(items)
    let actual = items |> Seq.sortBy (fun item -> item.Name)

    // assert
    actual
    |> Seq.zip expected
    |> Seq.forall (fun (old, act) ->
        old.Name = act.Name
        && old.Quality = act.Quality
        && old.SellIn = act.SellIn
    ) |@ $"{NewLine}expected: %A{expected |> Seq.map (|OldItem|)}"
       + $"{NewLine}actual: %A{actual |> Seq.map (|OldItem|)}"
