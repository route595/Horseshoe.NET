using System;
using System.Text;

namespace Horseshoe.NET.DataImport
{
    public static class TabularDataUtil
    {
        // A B  Y  Z AA AB AY AZ BA BB
        // 1 2 25 26 27 28 51 52 53 54

        // [ 0, 1] ->  A  [ 0, 26] ->  Z
        // [ 1, 1] -> AA  [ 1, 26] -> AZ
        // [ 2, 1] -> BA  [ 2, 26] -> BZ
        // ...
        // [26, 1] -> ZA  [26, 26] -> ZZ
        // [27, 1] -> [[1, 1], 1] -> AAA    [27, 26] -> [[1, 1], 26] -> AAZ

        // A = 65
        // B = 66
        // Y = 89
        // Z = 90

        /// <summary>
        /// Converts a column index to the Excel (a.k.a. "A1") style column label
        /// </summary>
        /// <param name="columnIndex">The column index to convert</param>
        /// <returns>An A1 style column label</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetA1StyleColumnLabel(int columnIndex)
        {
            if (columnIndex < 1)
                throw new ArgumentOutOfRangeException("A1 style column indexes may only be positive integers: " + columnIndex);

            var holder = new[] { 0, columnIndex };
            var strb = new StringBuilder();

            GetA1StyleColumnInternal(holder, strb);
            return strb.ToString();
        }

        private static void GetA1StyleColumnInternal(int[] holder, StringBuilder strb)
        {
            while (holder[1] > 26)
            {
                holder[0]++;
                holder[1] -= 26;
            }
            strb.Insert(0, (char)(holder[1] + 64));
            if (holder[0] > 26)
                GetA1StyleColumnInternal(new[] { 0, holder[0] }, strb);
            else if (holder[0] > 0)
                strb.Insert(0, (char)(holder[0] + 64));
        }
    }
}
