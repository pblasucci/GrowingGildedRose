namespace GildedRose
{
    /// Tracks the name of any inventory.
    public interface IInventoryItem
    {
        /// The name of a piece of inventory.
        string Name { get; }
    }

    /// An item with a constant value and no "shelf life".
    public sealed record Legendary(string Name, MagicQuality Quality = default) : IInventoryItem;

    /// Tracks any inventory which has both a value and a "shelf life".
    public interface IOrdinary : IInventoryItem
    {
        /// The value of a piece of inventory.
        Quality Quality { get; }

        /// The "shelf life" of a piece of inventory;
        /// when negative may impact the items quality.
        int SellIn { get; }
    }

    /// An item whose value decreases as its "shelf life" decreases.
    public sealed record Depreciating(string Name, Quality Quality, int SellIn) : IOrdinary;

    /// An item whose value increases as its "shelf life" decreases.
    public sealed record Appreciating(string Name, Quality Quality, int SellIn) : IOrdinary;

    /// An item whose value is subject to complex, "shelf life"-dependent rules.
    public sealed record BackstagePass(string Name, Quality Quality, int SellIn) : IOrdinary;

    /// Similar to a "Depreciating" item, but deteriorates twice as quickly.
    public sealed record Conjured(string Name, Quality Quality, int SellIn) : IOrdinary;
}
