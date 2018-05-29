using System.IO;
using Storage.Utils;

namespace Storage.Core
{
    public interface IVerifiable : ISerializable
    {
        void DeserializeUnsigned(BinaryReader reader);
        UInt160[] GetScriptHashesForVerifying();
        void SerializeUnsigned(BinaryWriter writer);
    }
}
