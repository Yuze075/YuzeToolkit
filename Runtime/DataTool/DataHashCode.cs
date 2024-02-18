#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace YuzeToolkit.DataTool
{
    public static class DataHashCode
    {
        public static int Get(string str) => str.Aggregate(5381, (current, c) => (current << 5) + current + c);

        public static int Get<T>(T t) where T : unmanaged => t switch
        {
            bool value => value ? 1 : 0,
            char value => value,
            sbyte value => value,
            byte value => value,
            short value => value,
            ushort value => value,
            int value => value,
            uint value => (int)value,
            float value => BitConverter.ToInt32(BitConverter.GetBytes(value), 0),
            long value => (int)value,
            ulong value => (int)value,
            double value => (int)BitConverter.ToInt64(BitConverter.GetBytes(value), 0),
            _ => BytesToHashCode(StructToBytes(t))
        };

        private static unsafe byte[] StructToBytes<T>(T t) where T : unmanaged
        {
            var size = sizeof(T);
            if (size == 0) return Array.Empty<byte>();
            var bytes = new byte[size];
            var ptr = &t;
            fixed (byte* currentPtr = bytes)
                Buffer.MemoryCopy(ptr, currentPtr, size, size);
            return bytes;
        }

        private static int BytesToHashCode(IReadOnlyList<byte> bytes) =>
            bytes.Count switch
            {
                0 => 0,
                1 => bytes[0],
                2 => (bytes[0] << 8) + bytes[1],
                3 => (bytes[0] << 16) + (bytes[1] << 8) + bytes[2],
                4 => (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3],
                _ => bytes.Aggregate(5381, (current, b) => (current << 5) + current + b)
            };
    }
}