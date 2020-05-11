using System.IO;
using Storage.Utils;

namespace Storage.Core
{
    public interface IVerifiable : ISerializable, IScriptContainer
    {

        Witness[] Scripts { get; set; }
        void DeserializeUnsigned(BinaryReader reader);
        UInt160[] GetScriptHashesForVerifying();
        void SerializeUnsigned(BinaryWriter writer);
    }
}
