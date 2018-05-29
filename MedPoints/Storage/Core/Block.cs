using System;
using System.IO;
using Storage.Cryptography;
using Storage.Network;
using Storage.Data;

namespace Storage.Core
{
    public class Block
    {
        public Transaction[] Transactions { get; set; }
        public Header Header { get; set; }
    }

    public class Header : IVerifiable, IEquatable<Header>
    {
        public uint Version;
        public UInt256 PrevHash;
        public UInt256 MerkleRoot;
        public uint Timestamp;
        public uint Index;
        public ulong ConsensusData;
        public UInt160 NextConsensus;

        private UInt256 _hash;

        public UInt256 Hash
        {
            get
            {
                if (_hash == null)
                {
                    _hash = new UInt256(CryptoHelper.Hash256(this.GetHashData()));
                }
                return _hash;
            }
        }

        public virtual int Size => sizeof(uint) + PrevHash.Size + MerkleRoot.Size + sizeof(uint) + sizeof(uint) + sizeof(ulong) + NextConsensus.Size + 1;

        public void Serialize(BinaryWriter writer)
        {
            ((IVerifiable)this).SerializeUnsigned(writer);
            writer.Write((byte)1);
        }

        public void Deserialize(BinaryReader reader)
        {
            ((IVerifiable)this).DeserializeUnsigned(reader);
            if (reader.ReadByte() != 1) throw new FormatException();

        }

        public void DeserializeUnsigned(BinaryReader reader)
        {
            Version = reader.ReadUInt32();
            PrevHash = reader.ReadSerializable<UInt256>();
            MerkleRoot = reader.ReadSerializable<UInt256>();
            Timestamp = reader.ReadUInt32();
            Index = reader.ReadUInt32();
            ConsensusData = reader.ReadUInt64();
            NextConsensus = reader.ReadSerializable<UInt160>();
        }

        public UInt160[] GetScriptHashesForVerifying()
        {
            throw new System.NotImplementedException();
        }

        public void SerializeUnsigned(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(PrevHash);
            writer.Write(MerkleRoot);
            writer.Write(Timestamp);
            writer.Write(Index);
            writer.Write(ConsensusData);
            writer.Write(NextConsensus);
        }

        public bool Equals(Header other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Hash.Equals(other.Hash);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Header);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }
    }

    public class Transaction
    {

    }
}
