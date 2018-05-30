using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Storage.Data.LevelDB
{
    public class Slice : IComparable<Slice>, IEquatable<Slice>
    {
        internal byte[] Buffer;

        public Slice()
        {
            
        }

        public Slice(IntPtr data, UIntPtr length)
        {
            Buffer = new byte[(int)length];
            Marshal.Copy(data, Buffer, 0, (int)length);
        }

        public int CompareTo(Slice other)
        {
            for (int i = 0; i < Buffer.Length && i < other.Buffer.Length; i++)
            {
                int r = Buffer[i].CompareTo(other.Buffer[i]);
                if (r != 0) return r;
            }
            return Buffer.Length.CompareTo(other.Buffer.Length);
        }

        public bool Equals(Slice other)
        {
            if (Buffer.Length != other.Buffer.Length) return false;
            return Buffer.SequenceEqual(other.Buffer);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (!(obj is Slice)) return false;
            return Equals((Slice)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int p = 16777619;
                var hash = (int)2166136261;

                for (int i = 0; i < Buffer.Length; i++)
                    hash = (hash ^ Buffer[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        public byte[] ToArray()
        {
            return Buffer ?? new byte[0];
        }

        unsafe public bool ToBoolean()
        {
            if (Buffer.Length != sizeof(bool))
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[0])
            {
                return *((bool*)pbyte);
            }
        }

        public byte ToByte()
        {
            if (Buffer.Length != sizeof(byte))
                throw new InvalidCastException();
            return Buffer[0];
        }

        unsafe public double ToDouble()
        {
            if (Buffer.Length != sizeof(double))
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[0])
            {
                return *((double*)pbyte);
            }
        }

        unsafe public short ToInt16()
        {
            if (Buffer.Length != sizeof(short))
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[0])
            {
                return *((short*)pbyte);
            }
        }

        unsafe public int ToInt32()
        {
            if (Buffer.Length != sizeof(int))
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[0])
            {
                return *((int*)pbyte);
            }
        }

        unsafe public long ToInt64()
        {
            if (Buffer.Length != sizeof(long))
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[0])
            {
                return *((long*)pbyte);
            }
        }

        unsafe public float ToSingle()
        {
            if (Buffer.Length != sizeof(float))
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[0])
            {
                return *((float*)pbyte);
            }
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Buffer);
        }

        unsafe public ushort ToUInt16()
        {
            if (Buffer.Length != sizeof(ushort))
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[0])
            {
                return *((ushort*)pbyte);
            }
        }

        unsafe public uint ToUInt32(int index = 0)
        {
            if (Buffer.Length != sizeof(uint) + index)
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[index])
            {
                return *((uint*)pbyte);
            }
        }

        unsafe public ulong ToUInt64()
        {
            if (Buffer.Length != sizeof(ulong))
                throw new InvalidCastException();
            fixed (byte* pbyte = &Buffer[0])
            {
                return *((ulong*)pbyte);
            }
        }

        public static implicit operator Slice(byte[] data)
        {
            return new Slice { Buffer = data };
        }

        public static implicit operator Slice(bool data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static implicit operator Slice(byte data)
        {
            return new Slice { Buffer = new[] { data } };
        }

        public static implicit operator Slice(double data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static implicit operator Slice(short data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static implicit operator Slice(int data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static implicit operator Slice(long data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static implicit operator Slice(float data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static implicit operator Slice(string data)
        {
            return new Slice { Buffer = Encoding.UTF8.GetBytes(data) };
        }

        public static implicit operator Slice(ushort data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static implicit operator Slice(uint data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static implicit operator Slice(ulong data)
        {
            return new Slice { Buffer = BitConverter.GetBytes(data) };
        }

        public static bool operator <(Slice x, Slice y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(Slice x, Slice y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(Slice x, Slice y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(Slice x, Slice y)
        {
            return x.CompareTo(y) >= 0;
        }

        public static bool operator ==(Slice x, Slice y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Slice x, Slice y)
        {
            return !x.Equals(y);
        }
    }
}
