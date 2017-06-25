using System;
using System.Linq.Expressions;

namespace siof.Common
{
    public static class Helpers
    {
        public static string GetPropertyName(this Expression<Func<object>> extension)
        {
            UnaryExpression unaryExpression = extension.Body as UnaryExpression;
            MemberExpression memberExpression = unaryExpression != null ?
                (MemberExpression)unaryExpression.Operand :
                (MemberExpression)extension.Body;

            return memberExpression.Member.Name;
        }
    }
}
