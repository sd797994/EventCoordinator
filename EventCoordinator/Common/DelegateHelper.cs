using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EventCoordinator.Common
{
    public class DelegateHelper
    {
        public static Func<TObj, Tin, Tout> CreateMethodDelegate<TObj, Tin, Tout>(MethodInfo method)
        {
            var mParameter = Expression.Parameter(typeof(TObj), "m");
            var pParameter = Expression.Parameter(typeof(Tin), "p");
            var mcExpression = Expression.Call(mParameter, method, Expression.Convert(pParameter, typeof(Tin)));
            var reExpression = Expression.Convert(mcExpression, typeof(Tout));
            return Expression.Lambda<Func<TObj, Tin, Tout>>(reExpression, mParameter, pParameter).Compile();
        }
    }
}
