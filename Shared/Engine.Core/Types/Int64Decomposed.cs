using System;
using System.Runtime.InteropServices;

namespace Engine.Core.Types
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Int64Decomposed
    {
        [FieldOffset(0)]
        public readonly Int64 Value;
        [FieldOffset(0)]
        public readonly byte Byte0;
        [FieldOffset(1)]
        public readonly byte Byte1;
        [FieldOffset(2)]
        public readonly byte Byte2;
        [FieldOffset(3)]
        public readonly byte Byte3;
        [FieldOffset(4)]
        public readonly byte Byte4;
        [FieldOffset(5)]
        public readonly byte Byte5;
        [FieldOffset(6)]
        public readonly byte Byte6;
        [FieldOffset(7)]
        public readonly byte Byte7;

        public Int64Decomposed(Int64 value)
        {
            Byte0 = Byte1 = Byte2 = Byte3 = Byte4 = Byte5 = Byte6 = Byte7 = 0;
            Value = value;
        }

        public Int64Decomposed(byte[] bytes)
        {
            Verify.IsNotNull(bytes, nameof(bytes));
            Verify.IsEqual(bytes.Length, 8, nameof(bytes.Length));
            Value = 0;
            Byte0 = bytes[0];
            Byte1 = bytes[1];
            Byte2 = bytes[2];
            Byte3 = bytes[3];
            Byte4 = bytes[4];
            Byte5 = bytes[5];
            Byte6 = bytes[6];
            Byte7 = bytes[7];
        }

        public static implicit operator byte[] (Int64Decomposed value)
        {
            return new byte[] {
                value.Byte0,
                value.Byte1,
                value.Byte2,
                value.Byte3,
                value.Byte4,
                value.Byte5,
                value.Byte6,
                value.Byte7
            };
        }

        public static implicit operator Int64 (Int64Decomposed value)
        {
            return value.Value;
        }
    }
}
