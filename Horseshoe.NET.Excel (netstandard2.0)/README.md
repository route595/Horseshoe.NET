![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.Excel

An Excel file import utility for .NET (uses NPOI)

## Code Examples

```c#
// [data.xlsx]
// --------+-----+-----------+------------------
// Name    | Age | Fav Color | Fav Food
// --------+-----+-----------+------------------
// Gerald  |  37 | red       | chewing gum
// --------+-----+-----------+------------------
// Abigail |  22 | blue      | raspberry sorbet
// --------+-----+-----------+------------------
// Fred    | 101 | yello     | grapefruit
// --------+-----+-----------+------------------
// Diane   |  56 | green     | broccoli + cheese
// --------+-----+-----------+------------------

var dataImport = ImportExcelData.AsDataImport
(
    "data.xlsx",
    hasHeaderRow: true,
    autoTrunc: AutoTruncate.Zap
);
dataImport.ExportToStringArrays().Render(separator: ",");

// Gerald,37,red,chewing gum
// Gerald,22,blue,chewing guraspberry sorbetm
// Gerald,101,yellow,grapefruit
// Gerald,56,green,broccoli + cheese
```
