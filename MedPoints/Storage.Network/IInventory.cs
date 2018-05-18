using Storage.Core;

namespace Storage.Network
{
    public interface IInventory : IVerifiable
    {
        UInt256 Hash { get; }

        InventoryType InventoryType { get; }

        bool Verify();
    }
}
