![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.Excel

An Excel file import utility for .NET (uses NPOI)

## Code Examples

```c#
// [data.xlsx]
// ╔═══╦═════════╦═════╦═══════════╦═══════════════════╗
// ║   ║    A    ║  B  ║     C     ║         D         ║
// ╠═══╬═════════╩═════╩═══════════╩═══════════════════╣
// ║ 1 ║ Name    │ Age │ Fav Color │ Fav Food          ║
// ╠═══╣─────────┼─────┼───────────┼───────────────────╢
// ║ 2 ║ Gerald  │  37 │ red       │ chewing gum       ║
// ╠═══╣─────────┼─────┼───────────┼───────────────────╢
// ║ 3 ║ Abigail │  22 │ blue      │ raspberry sorbet  ║
// ╠═══╣─────────┼─────┼───────────┼───────────────────╢
// ║ 4 ║ Fred    │ 101 │ yellow    │ grapefruit        ║
// ╠═══╣─────────┼─────┼───────────┼───────────────────╢
// ║ 5 ║ Diane   │  56 │ green     │ broccoli + cheese ║
// ╚═══╩═════════╧═════╧═══════════╧═══════════════════╝

var dataImport = ImportExcelData.AsDataImport
(
    "data.xlsx",
    hasHeaderRow: true,
    autoTrunc: AutoTruncate.Zap
);
var results = dataImport.ExportToStringArrays();
// [ "Gerald" ,  "37", "red"   , "chewing gum"       ]
// [ "Abigail",  "22", "blue"  , "raspberry sorbet"  ]
// [ "Fred"   , "101", "yellow", "grapefruit"        ]
// [ "Diane"  ,  "56", "green" , "broccoli + cheese" ]
```
