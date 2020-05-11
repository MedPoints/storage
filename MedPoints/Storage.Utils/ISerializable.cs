using System.IO;

namespace Storage.Utils
{
    public interface ISerializable
    {
        int Size { get; }
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}
