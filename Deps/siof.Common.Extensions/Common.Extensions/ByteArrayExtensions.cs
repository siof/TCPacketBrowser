using System.Text;

namespace siof.Common.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToUTF8String(this byte[] array)
        {
            return array.IfNotNull(arr => Encoding.UTF8.GetString(array), null);
        }
    }
}
