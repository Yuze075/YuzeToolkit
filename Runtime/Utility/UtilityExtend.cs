using System;
using System.Runtime.InteropServices;

namespace YuzeToolkit
{
    public static class UtilityExtend
    {
        public static int GetFixedHashCode(this string str)
        {
            var hash = 5381;
            var length = str.Length;
            for (var i = 0; i < length; i++)
            {
                hash = (hash << 5) + hash + str[i];
            }
            return hash;
        }

        public static int GetFixedHashCode(this byte[] bytes)
        {
            var hash = 5381;
            var length = bytes.Length;
            for (var i = 0; i < length; i++)
            {
                hash = (hash << 5) + hash + bytes[i];
            }

            return hash;
        }

        public static int GetFixedHashCode<T>(this T t) where T : unmanaged
        {
            return t switch
            {
                bool value => value ? 1 : 0,
                char value => value,
                sbyte value => value,
                byte value => value,
                short value => value,
                ushort value => value,
                int value => value,
                uint value => value.StructToBytes().GetFixedHashCode(),
                long value => value.StructToBytes().GetFixedHashCode(),
                ulong value => value.StructToBytes().GetFixedHashCode(),
                nint value => value.StructToBytes().GetFixedHashCode(),
                nuint value => value.StructToBytes().GetFixedHashCode(),
                float value => value.StructToBytes().GetFixedHashCode(),
                double value => value.StructToBytes().GetFixedHashCode(),
                decimal value => value.StructToBytes().GetFixedHashCode(),
                Enum value => value.ToString().GetFixedHashCode(),
                _ => t.StructToBytes().GetFixedHashCode()
            };
        }

        private static byte[] StructToBytes<T>(this T data) where T : unmanaged
        {
            //计算对象长度
            var arrayLength = Marshal.SizeOf(typeof(T));
            //根据长度定义一个数组
            var dataBytes = new byte[arrayLength];

            //在非托管内存中分配一段iAryLen大小的空间
            var ptr = Marshal.AllocHGlobal(arrayLength);
            //将托管内存的东西发送给非托管内存上
            Marshal.StructureToPtr(data, ptr, true);
            //将bytes组数Copy到Ptr对应的空间中
            Marshal.Copy(ptr, dataBytes, 0, arrayLength);
            //释放非托管内存
            Marshal.FreeHGlobal(ptr);
            return dataBytes;
        }
    }
}