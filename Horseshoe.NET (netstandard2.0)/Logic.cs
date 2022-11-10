using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET
{
    public static class Logic
    {
        public static object If(bool condition, Func<object> evaluateIfTrue, Func<object> evaluateIfFalse)
        {
            if (condition)
            {
                return evaluateIfTrue.Invoke();
            }
            return evaluateIfFalse.Invoke();
        }

        public static E If<E>(bool condition, Func<E> evaluateIfTrue, Func<E> evaluateIfFalse)
        {
            if (condition)
            {
                return evaluateIfTrue.Invoke();
            }
            return evaluateIfFalse.Invoke();
        }
    }
}
