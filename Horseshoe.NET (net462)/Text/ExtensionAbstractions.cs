using System;
using System.Text;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A collection of extension methods for <c>string</c> and <c>char</c> interpretation and <c>string</c> building and manipulation.
    /// </summary>
    public static class ExtensionAbstractions
    {
        /// <summary>
        /// <para>
        /// Contitionally appends a value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </para>
        /// <para>
        /// WARNING: When supplying expressions instead of values, unwanted results may occur including runtime errors.
        /// </para>
        /// For example:
        /// <para>
        /// <code>
        /// // If x == 0 a runtime 'divide by zero' exception will occur because although
        /// // only the 'y' expression is being appended the 'y / x' expression is still being evaluated.
        /// var strb = new StringBuilder("result = ")
        ///     .AppendIf(x > 0, y / x, y);
        ///  
        /// // Instead, try one of the following...
        /// 
        /// // Use the overloaded version of this method and embed the expressions in lambdas 
        /// // which will be evaluated based on the conditional expression.
        /// var strb = new StringBuilder("result = ")
        ///     .AppendIf&lt;int&gt;(x > 0, () => y / x, () => y); 
        ///  
        /// // Alternatively, simply use the ternary conditional operator. The 'divide by zero' exception
        /// // is prevented by short circuiting the 'y / x' expression when x == 0.
        /// var strb = new StringBuilder("result = ")
        ///     .Append(x > 0 ? y / x : y); 
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="sb">A <c>StringBuilder</c> instance</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The value to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, object valueIfTrue, object valueIfFalse = null)
        {
            if (condition)
                sb.Append(valueIfTrue);
            else if (valueIfFalse != null)
                sb.Append(valueIfFalse);
            return sb;
        }

        /// <summary>
        /// Contitionally appends a function derived value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is evaluated and appended.
        /// </summary>
        /// <param name="sb">A <c>StringBuilder</c> instance</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The function to evaluate if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional function to evaluate if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendIf<T>(this StringBuilder sb, bool condition, Func<T> valueIfTrue, Func<T> valueIfFalse = null)
        {
            if (condition)
                sb.Append(valueIfTrue.Invoke());
            else if (valueIfFalse != null)
                sb.Append(valueIfFalse.Invoke());
            return sb;
        }

        /// <summary>
        /// <para>
        /// Contitionally appends a line of text to a <c>StringBuilder</c>.  If <c>condition == false</c> then no line is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended as a line.
        /// </para>
        /// <para>
        /// WARNING: When supplying expressions instead of values, unwanted results may occur including runtime errors.
        /// </para>
        /// For example:
        /// <para>
        /// <code>
        /// // If x == 0 a runtime 'divide by zero' exception will occur because although
        /// // the 'y' expression is being appended the 'y / x' expression is still being evaluated.
        /// var strb = new StringBuilder("result:")
        ///     .AppendLineIf(x > 0, (y / x).ToString(), y.ToString());
        ///  
        /// // Instead, try one of the following...
        /// 
        /// // Use the overloaded version of this method and embed the expressions in lambdas 
        /// // which will be evaluated based on the conditional expression.
        /// var strb = new StringBuilder("result = ")
        ///     .AppendLineIf&lt;int&gt;(x > 0, () => (y / x).ToString(), () => y.ToString()); 
        ///  
        /// // Alternatively, simply use the ternary conditional operator. The 'divide by zero' exception
        /// // is prevented by short circuiting the 'y / x' expression when x == 0.
        /// var strb = new StringBuilder("result:")
        ///     .AppendLine((x > 0 ? y / x : y).ToString()); 
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="sb">A <c>StringBuilder</c> instance</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The value to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, string valueIfTrue, string valueIfFalse = null)
        {
            if (condition)
                sb.AppendLine(valueIfTrue);
            else if (valueIfFalse != null)
                sb.AppendLine(valueIfFalse);
            return sb;
        }

        /// <summary>
        /// Contitionally appends a function derived line of text to a <c>StringBuilder</c>.  If <c>condition == false</c> then no line is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is evaluated and appended as a line.
        /// </summary>
        /// <param name="sb">A <c>StringBuilder</c> instance</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The value to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, Func<string> valueIfTrue, Func<string> valueIfFalse = null)
        {
            if (condition)
                sb.AppendLine(valueIfTrue?.Invoke());
            else if (valueIfFalse != null)
                sb.AppendLine(valueIfFalse.Invoke());
            return sb;
        }

        /// <summary>
        /// <para>
        /// Contitionally inserts a value into a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is inserted 
        /// unless <c>valueIfFalse</c> has a value, then that is inserted.
        /// </para>
        /// <para>
        /// WARNING: When supplying expressions instead of values, unwanted results may occur including runtime errors.
        /// </para>
        /// For example:
        /// <para>
        /// <code>
        /// // If x == 0 a runtime 'divide by zero' exception will occur because although
        /// // only the 'y' expression is being inserted the 'y / x' expression is still being evaluated.
        /// var strb = new StringBuilder(" is the result")
        ///     .InsertIf(0, x > 0, y / x, y);
        ///  
        /// // Instead, try one of the following...
        /// 
        /// // Use the overloaded version of this method and embed the expressions in lambdas 
        /// // which will be evaluated based on the conditional expression.
        /// var strb = new StringBuilder(" is the result")
        ///     .InsertIf&lt;int&gt;(0, x > 0, () => y / x, () => y);  // overloaded function
        ///  
        /// // Alternatively, simply use the ternary conditional operator. The 'divide by zero' exception
        /// // is prevented by short circuiting the 'y / x' expression when x == 0.
        /// var strb = new StringBuilder(" is the result")
        ///     .Insert(0, x > 0 ? y / x : y); 
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="sb">A <c>StringBuilder</c> instance</param>
        /// <param name="index">The position in this instance where the insertion begins</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The value to insert if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to insert if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder InsertIf(this StringBuilder sb, int index, bool condition, object valueIfTrue, object valueIfFalse = null)
        {
            if (condition)
                sb.Insert(index, valueIfTrue);
            else if (valueIfFalse != null)
                sb.Insert(index, valueIfFalse);
            return sb;
        }

        /// <summary>
        /// Contitionally inserts a function derived value into a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is inserted 
        /// unless <c>valueIfFalse</c> has a value, then that is evaluated and inserted.
        /// </summary>
        /// <param name="sb">A <c>StringBuilder</c> instance</param>
        /// <param name="index">The position in this instance where the insertion begins</param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The function to evaluate if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional function to evaluate if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder InsertIf<T>(this StringBuilder sb, int index, bool condition, Func<T> valueIfTrue, Func<T> valueIfFalse = null)
        {
            if (condition)
                sb.Insert(index, valueIfTrue.Invoke());
            else if (valueIfFalse != null)
                sb.Insert(index, valueIfFalse.Invoke());
            return sb;
        }
    }
}
