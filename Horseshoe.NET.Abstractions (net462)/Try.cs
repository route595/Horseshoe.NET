using System;

namespace Horseshoe.NET
{
    public static class Try
    {
        public static void Invoke<TResult>(Func<TResult> func, out TResult result, TResult defaultValue = default)
        {
            try
            {
                result = func.Invoke();
            }
            catch
            {
                result = defaultValue;
            }
        }

        public static void Invoke<T1, TResult>(Func<T1, TResult> func, T1 t1, out TResult result, TResult defaultValue = default)
        {
            try
            {
                result = func.Invoke(t1);
            }
            catch
            {
                result = defaultValue;
            }
        }

        public static void Invoke<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 t1, T2 t2, out TResult result, TResult defaultValue = default)
        {
            try
            {
                result = func.Invoke(t1, t2);
            }
            catch
            {
                result = defaultValue;
            }
        }

        public static void Invoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, out TResult result, TResult defaultValue = default)
        {
            try
            {
                result = func.Invoke(t1, t2, t3);
            }
            catch
            {
                result = defaultValue;
            }
        }
    }
}
