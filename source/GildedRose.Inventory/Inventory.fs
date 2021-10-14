module GildedRose.Inventory

[<Measure>] type days


[<Struct>]
type Quality =
  private { Value : uint8 }
  override me.ToString() = $"%s{nameof Quality} %02i{me.Value}"

  static member MinValue = Quality.Of(00uy)
  static member MaxValue = Quality.Of(50uy)

  static member Of(value) = { Value = min value 50uy }
  static member op_Explicit(quality) = quality.Value

  static member ( + ) (left, right) =
    let sum = left.Value + right.Value
    if sum < left.Value then Quality.MaxValue else Quality.Of(sum)

  static member ( - ) (left, right) =
    let dif = left.Value - right.Value
    if left.Value < dif then Quality.MinValue else Quality.Of(dif)


[<Struct>]
type MagicQuality =
  override me.ToString() = $"%s{nameof MagicQuality} %02i{uint8 me}"
  static member op_Explicit(_ : MagicQuality) = 80uy


type Item =
  | Legendary     of name : string * quality : MagicQuality
  | Depreciating  of name : string * quality : Quality * sellIn : int32<days>
  | Conjured      of name : string * quality : Quality * sellIn : int32<days>
  | Appreciating  of name : string * quality : Quality * sellIn : int32<days>
  | BackstagePass of name : string * quality : Quality * sellIn : int32<days>


[<CompiledName("UpdateItem")>]
let updateItem item =
  // advance the "shelf life" clock by a single day
  let (|Aged|) sellIn = Aged(sellIn - 1<days>)

  // items with negative "shelf life" gain/lose value twice as quickly
  let rateOfChange sellIn = if sellIn < 0<days> then 2uy else 1uy

  match item with
  | Legendary _ -> item

  | Depreciating (name, quality, Aged sellIn') ->
      let quality' = quality - Quality.Of(rateOfChange sellIn')
      Depreciating(name, quality', sellIn')

  | Conjured (name, quality, Aged sellIn') ->
      let quality' = quality - Quality.Of(2uy * rateOfChange sellIn')
      Conjured(name, quality', sellIn')

  | Appreciating (name, quality, Aged sellIn') ->
      let quality' = quality + Quality.Of(rateOfChange sellIn')
      Appreciating(name, quality', sellIn')

  | BackstagePass (name, quality, sellIn & Aged sellIn') ->
      let quality' =
        if sellIn' < 0<days> then
          Quality.MinValue
        else
          //  NOTE
          //  ----
          //  Pass quality has a "hard cliff", based on "shelf life".
          //  However, until then, its value is calculated against
          //  the _current_ expiry (i.e. before advancing the clock).
          quality + Quality.Of(
            match sellIn with
            | days when days <=  5<days> -> 3uy
            | days when days <= 10<days> -> 2uy
            | _                          -> 1uy
          )
      BackstagePass(name, quality', sellIn')
