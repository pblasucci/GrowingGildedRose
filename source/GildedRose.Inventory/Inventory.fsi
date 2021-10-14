/// Maintains inventory for The Gilded Rose.
module GildedRose.Inventory

/// A common unit of time (n.b. "business day" -- not necessarily a "solar day").
[<Measure>] type days


/// The value of a ordinary item
/// (n.b. constrained within: 0 .. 50 "units", inclusive).
[<Struct>]
type Quality =
  private { Value : uint8 }

  /// The smallest possible value of a Quality (0 "units").
  static member MinValue : Quality

  /// The largest possible value of a Quality (50 "units").
  static member MaxValue : Quality

  /// Constructs a Quality from the given value
  /// (n.b. overlarge inputs are truncated to Quality.MaxValue).
  static member Of : value : uint8 -> Quality

  /// Defines an explicit conversion of a Quality to an unsigned 8-bit integer.
  static member op_Explicit : Quality -> uint8

  /// Adds two Quality values
  /// (n.b. result does not overflow, but is truncated to Quality.MaxValue).
  static member ( + ) : left : Quality * right : Quality -> Quality

  /// Subtracts two Quality values
  /// (n.b. result does not underflow, but is truncated to Quality.MinValue).
  static member ( - ) : left : Quality * right : Quality -> Quality


/// The value of an extraordinary item (80 "units", always).
[<Struct>]
type MagicQuality =
  /// Defines an explicit conversion of a MagicQuality to an unsigned 8-bit integer.
  static member op_Explicit : MagicQuality -> uint8


/// Tracks the category, name, value, and "shelf life" of any inventory.
type Item =
  /// An item with a constant value and no "shelf life".
  | Legendary of name : string * quality : MagicQuality

  /// An item whose value decreases as its "shelf life" decreases.
  | Depreciating of name : string * quality : Quality * sellIn : int32<days>

  /// An item whose value increases as its "shelf life" decreases.
  | Appreciating  of name : string * quality : Quality * sellIn : int32<days>

  /// An item whose value is subject to complex, "shelf life"-dependent rules.
  | BackstagePass of name : string * quality : Quality * sellIn : int32<days>


/// Change the quality and "shelf life" for an Item
/// (i.e. apply appropriate rules for the passage of a single "business day").
val updateItem : item : Item -> Item
