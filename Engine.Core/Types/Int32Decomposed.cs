using System;
using System.Runtime.InteropServices;

namespace Engine.Core.Types
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Int32Decomposed
    {
        [FieldOffset(0)]
        public readonly Int32 Value;
        [FieldOffset(0)]
        public readonly byte Byte0;
        [FieldOffset(1)]
        public readonly byte Byte1;
        [FieldOffset(2)]
        public readonly byte Byte2;
        [FieldOffset(3)]
        public readonly byte Byte3;

        public Int32Decomposed(Int32 value)
        {
            Byte0 = Byte1 = Byte2 = Byte3 = 0;
            Value = value;
        }

        public Int32Decomposed(byte[] bytes)
        {
            Verify.IsNotNull(bytes, nameof(bytes));
            Verify.IsEqual(bytes.Length, 4, nameof(bytes.Length));
            Value = 0;
            Byte0 = bytes[0];
            Byte1 = bytes[1];
            Byte2 = bytes[2];
            Byte3 = bytes[3];
        }

        public static implicit operator byte[] (Int32Decomposed value)
        {
            return new byte[] {
                value.Byte0,
                value.Byte1,
                value.Byte2,
                value.Byte3
            };
        }

        public static implicit operator Int32 (Int32Decomposed value)
        {
            return value.Value;
        }
    }
}
