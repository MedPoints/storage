// ReSharper disable InconsistentNaming
namespace Storage.Network
{
    public enum InventoryType : byte
    {
        TX = 0x01,
        Block = 0x02,
        Consensus = 0xe0
    }
}
