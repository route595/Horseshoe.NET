using NPOI.SS.UserModel;

namespace Horseshoe.NET.Excel
{
    public static class Extensions
    {
        public static bool IsBlank(this IRow row)
        {
            if (row.LastCellNum > 0)
            {
                foreach (var cell in row.Cells)
                {
                    if (!IsBlank(cell)) return false;
                }
            }
            return true;
        }

        public static bool IsBlank(this ICell cell)
        {
            if (cell.CellType == CellType.Blank) 
                return true;
            var cellType = cell.CellType == CellType.Formula
                ? cell.CachedFormulaResultType
                : cell.CellType;
            return cellType == CellType.String && Zap.String(cell.StringCellValue) == null;
        }
    }
}
