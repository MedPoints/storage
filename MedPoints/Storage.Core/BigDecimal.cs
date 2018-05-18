using System;
using System.Numerics;

namespace Storage.Core
{
    public struct BigDecimal
    {
        private readonly BigInteger _value;
        private readonly byte _decimals;

        public BigInteger Value => _value;
        public byte Decimals => _decimals;
        public int Sign => _value.Sign;

        public BigDecimal(BigInteger value, byte decimals)
        {
            this._value = value;
            this._decimals = decimals;
        }

        public BigDecimal ChangeDecimals(byte decimals)
        {
            if (_decimals == decimals) return this;
            BigInteger value;
            if (_decimals < decimals)
            {
                value = this._value * BigInteger.Pow(10, decimals - this._decimals);
            }
            else
            {
                BigInteger divisor = BigInteger.Pow(10, this._decimals - decimals);
                value = BigInteger.DivRem(this._value, divisor, out BigInteger remainder);
                if (remainder > BigInteger.Zero)
                    throw new ArgumentOutOfRangeException();
            }
            return new BigDecimal(value, decimals);
        }

        public static BigDecimal Parse(string s, byte decimals)
        {
            if (!TryParse(s, decimals, out BigDecimal result))
                throw new FormatException();
            return result;
        }

        public Fixed8 ToFixed8()
        {
            try
            {
                return new Fixed8((long)ChangeDecimals(8)._value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(ex.Message, ex);
            }
        }

        public override string ToString()
        {
            BigInteger divisor = BigInteger.Pow(10, _decimals);
            BigInteger result = BigInteger.DivRem(_value, divisor, out BigInteger remainder);
            if (remainder == 0) return result.ToString();
            return $"{result}.{remainder.ToString("d" + _decimals)}".TrimEnd('0');
        }

        public static bool TryParse(string s, byte decimals, out BigDecimal result)
        {
            int e = 0;
            int index = s.IndexOfAny(new[] { 'e', 'E' });
            if (index >= 0)
            {
                if (!sbyte.TryParse(s.Substring(index + 1), out sbyte e_temp))
                {
                    result = default(BigDecimal);
                    return false;
                }
                e = e_temp;
                s = s.Substring(0, index);
            }
            index = s.IndexOf('.');
            if (index >= 0)
            {
                s = s.TrimEnd('0');
                e -= s.Length - index - 1;
                s = s.Remove(index, 1);
            }
            int ds = e + decimals;
            if (ds < 0)
            {
                result = default(BigDecimal);
                return false;
            }
            if (ds > 0)
                s += new string('0', ds);
            if (!BigInteger.TryParse(s, out BigInteger value))
            {
                result = default(BigDecimal);
                return false;
            }
            result = new BigDecimal(value, decimals);
            return true;
        }
    }
}
