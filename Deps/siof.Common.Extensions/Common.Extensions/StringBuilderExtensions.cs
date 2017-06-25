using System.Text;

namespace siof.Common.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineFormat(this StringBuilder builder, string format, params object[] values)
        {
            builder.AppendFormat(format, values);
            builder.AppendLine();

            return builder;
        }
    }
}
