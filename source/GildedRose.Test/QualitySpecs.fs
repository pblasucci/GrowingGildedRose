namespace GildedRose.Test

(* test infrastructure *)
open FsCheck
open FsCheck.Xunit

open type System.Environment

(* system under test *)
open GildedRose.Inventory

/// Evaluates whether or not instance of the Quality type
/// upholds certain invariants (based on given requirements).
[<Properties(Arbitrary=[|typeof<InventoryGenerators>|])>]
module QualitySpecs =
  [<Property>]
  let ``construction truncates high`` raw =
    let quality = Quality.Of(raw)
    if raw < 50uy then raw = byte quality else quality = Quality.MaxValue
    |@ $"raw: %i{raw}, quality: %A{quality}"

  [<Property>]
  let ``addition truncates high`` raw1 raw2 =
    let quality1 = Quality.Of(raw1)
    let quality2 = Quality.Of(raw2)
    let quality' = quality1 + quality2

    let raw' = raw1 + raw2
    if raw' < raw1 then quality' = Quality.MaxValue else true
    |@ $"raw sum: %i{raw'}, quality sum: %A{quality'}"

  [<Property>]
  let ``additive identity holds`` quality =
    quality + Quality.MinValue = quality

  [<Property>]
  let ``addition is commutative`` (quality1 : Quality) (quality2 : Quality) =
    quality1 + quality2 = quality2 + quality1

  [<Property>]
  let ``addition is associative``
    (quality1 : Quality)
    (quality2 : Quality)
    (quality3 : Quality)
    =
    quality1 + (quality2 + quality3) = (quality1 + quality2) + quality3

  [<Property>]
  let ``subtraction truncates low`` raw1 raw2 =
    let quality1 = Quality.Of(raw1)
    let quality2 = Quality.Of(raw2)
    let quality' = quality1 - quality2

    let raw' = raw1 - raw2
    if raw1 < raw' then quality' = Quality.MinValue else true
    |@ $"raw difference: %i{raw'}, quality difference: %A{quality'}"

  [<Property>]
  let ``subtractive identity holds`` quality =
    (quality - Quality.MinValue = quality)
    |@ $"0 on the right (%A{quality})"
    .&.
    (Quality.MinValue - quality = Quality.MinValue)
    |@ $"0 on the left (%A{quality})"
