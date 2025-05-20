![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-256x256.png)

# Horseshoe.NET

A suite of .NET utilities with a dual purpose: simplify coding tasks and offer a variety of unique capabilities for developers. What this represents is a career's worth of collecting useful code snippets packaged as a NuGet library that any .NET developer can utilize.  

A large portion of this code base is dedicated to replacing verbose, repetitive boilerplate code for tasks such as configuration, ADO.NET / ODP.NET database queries and web API calls with highly customizable single line code replacements.

## Packages and Namespaces
- (NuGet package)
  - (more namespaces)
- Horseshoe.NET
  - Horseshoe.NET.Bootstrap
  - Horseshoe.NET.Collections
  - Horseshoe.NET.Comparison   (introducing the 'Criterinator' search tool)
  - Horseshoe.NET.ConsoleX
    - Horseshoe.NET.ConsoleX.Plugins
  - Horseshoe.NET.Crypto
  - Horseshoe.NET.Data
  - Horseshoe.NET.DataImport
  - Horseshoe.NET.DateAndTime
  - Horseshoe.NET.Db
  - Horseshoe.NET.Dotnet
  - Horseshoe.NET.Email
    - Horseshoe.NET.Email.EmailToSms
  - Horseshoe.NET.IO
    - Horseshoe.NET.IO.FileFilter
    - Horseshoe.NET.IO.FileTraversal
    - Horseshoe.NET.Ftp
  - Horseshoe.NET.ObjectsTypesAndValues
  - Horseshoe.NET.OleDb  (.NET Framework only)
  - Horseshoe.NET.Primitives
  - Horseshoe.NET.RelayMessages
  - Horseshoe.NET.Text
    - Horseshoe.NET.Text.TextGrid
  - Horseshoe.NET.Xml
    - Horseshoe.NET.Xml.Doc
- Horseshoe.NET.ActiveDirectory
- Horseshoe.NET.Caching
- Horseshoe.NET.Caching.Abstractions (namespace Horseshoe.NET.Caching)
- Horseshoe.NET.Configuration
- Horseshoe.NET.Excel
- Horseshoe.NET.Finance
- Horseshoe.NET.Http
  - Horseshoe.NET.Http.Mvc
  - Horseshoe.NET.Http.ReportingServices
  - Horseshoe.NET.Http.WebForms (.NET Framework only)
- Horseshoe.NET.Jwt
- Horseshoe.NET.Jwt.Abstractions (namespace Horseshoe.NET.Jwt)
- Horseshoe.NET.Odbc
- Horseshoe.NET.OracleDb
- Horseshoe.NET.SecureIO
- Horseshoe.NET.SqlDb

## Code Examples

#### Horseshoe.NET.ActiveDirectory

```c#
var userIdNameOrEmail = "djones1" -or- "david.jones@mybiz.com" -or- "Jones, David E. [Contractor]";
var ldapProperties = "name|cn|samaccountname|memberof";   // (optional)

var user = ADUtil.GetUser(userIdNameOrEmail, propertiesToLoad: ldapProperties);
-or-
var user = ADUtil.Authenticate(userIdNameOrEmail, "P@$$W0rd123", propertiesToLoad: ldapProperties);

user.GetGroups(orderByAdsPath: true);        // "All Contractors", "DB Admins", "Hiring Team"...
user.OU;                                     // "Contractors"
user.RawOU;                                  // LDAP://OU=Contractors,DC=mybiz,DC=com
ADUtil.ListOUs();                            // (lists all OUs in Active Directory)
ADEngine.GetDomainContext().ConnectedServer; // (lists name of connected domain controller)
```

#### Horseshoe.NET.Comparison    (introducing the 'Criterinator' search tool)

```c#
var filteredEmployees = SearchEmployeesByLastName("Finkleman", ignoreCase: true);

IEnumerable<Employee> SearchEmployeesByLastName(ICriterinator<string> lastNameCriterinator)
{
    return allEmployees.Where(e => lastNameCriterinator.IsMatch(e.LastName))
}

IEnumerable<Employee> SearchEmployeesByLastNameContains(string lastNameOrPartial, bool ignoreCase = false)
{
    return ignoreCase 
        ? SearchEmployeesByLastName(Criterinator.ContainsIgnoreCase(lastNameOrPartial))
        : SearchEmployeesByLastName(Criterinator.Contains(lastNameOrPartial))
}
```

#### Horseshoe.NET.Configuration

```xml
<appSettings>
  <add key="myInt" value="90" />
  <add key="myHexInt" value="5a" />
  <add key="myHexInt_Annotation" value="5a[hex]" />
  <add key="myHexInt_Format" value="0x5a" />
</appSettings>
```

```c#
// Getting an int...
Config.Get<int>("myInt");                // value="90" -> 90

// Getting a hex formatted int...
Config.Get<int>("myHexInt",              // value="5a" -> 90
    numberStyle: NumberStyles.HexNumber);

// Getting a hex formatted int w/ key annotation...
Config.Get<int>("myHexInt[hex]");        // value="5a" -> 90

// Getting a hex formatted int w/ data annotation or data format...
Config.Get<int>("myHexInt_Annotation");  // value="5a[hex]" -> 90
Config.Get<int>("myHexInt_Format");      // value="0x5a" -> 90

// Before Horseshoe.NET... 
var stringValue = ConfigurationManager.AppSettings["myInt"];
if (stringValue != null)
    return int.Parse(stringValue);
- or -
var stringValue = ConfigurationManager.AppSettings["myHexInt"];
if (stringValue != null)
    return int.Parse(stringValue, NumberStyles.HexNumber);
```

#### Horseshoe.NET.ConsoleX

```c#
Console.WriteLine("Select your preferred day(s) for playing pickleball.");
var daysOfWeek = new [] { "Sunday", "Monday", ... , "Friday", "Saturday" };
result = PromptX.List(daysOfWeek, selectionMode: ListSelectionMode.ZeroOrMore);
result.selectedIndices   // [ 2, 3, 4 ]
result.selectedItems     // [ "Monday", "Tuesday", "Wednesday" ]

// console input/output        
[1] Sunday
[2] Monday
[3] Tuesday
[4] Wednesday
[5] Thursday
[6] Friday
[7] Saturday
> 0               // ** 0 is outside the allowed range: 1 to 7 **
                  // Press any key to try again…
> 2               // [ "Monday" ]
> 2-4,6           // [ "Monday", "Tuesday", "Wednesday", "Friday" ]
> all             // [ "Sunday", "Monday",      ...    , "Friday", "Saturday" ]
> all except 1,7  // [ "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" ]
> none (or blank) // [ ]
```

#### Horseshoe.NET.Crypto

```c#
var plaintext = "H1ghW@y2Hev3n";
var b64Options = new CryptoOptions { IsCiphertextBase64Encoded = true };
ciphertext = Encrypt.String(plaintext, b64Options);  // "2puPR6R9//bo/D3hK+bONQ=="
plaintext = Decrypt.String(ciphertext, b64Options);  // "H1ghW@y2Hev3n"
```

#### Horseshoe.NET.Excel

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
// ║ 4 ║ Fred    │ 101 │ yello     │ grapefruit        ║
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

#### Horseshoe.NET.Finance

```c#
FinanceEngine
    .GenerateCreditPayoffProjection(accounts, snowballing: true, extraSnowballAmount: 100m)
    .RenderToTextGrid();

// Date     │ Line of Credit                   │ Credit Card                       │ Totals                 
//          │ APR: 6.24% ($20.80)              │ Acct #: *7890                     │                        
//          │ Balance: $4,000.00               │ APR: 12.99% ($108.25)             │                        
//          │ Min Payment: $150.00             │ Balance: $10,000.00               │                        
//          │                                  │ Min Payment: $200.00              │                        
// ----     │ --------------------             │ ---------------------             │ ------                 
//          │ Pmt     Pri     Int    Bal       │ Pmt     Pri     Int     Bal       │ Pmt     Pri     Int    
// Feb 2025 │ $250.00 $229.20 $20.80 $3,770.80 │ $200.00 $ 91.75 $108.25 $9,908.25 │ $450.00 $320.95 $129.05
// Mar 2025 │ $250.00 $230.39 $19.61 $3,540.41 │ $200.00 $ 92.74 $107.26 $9,815.51 │ $450.00 $323.13 $126.87
// Apr 2025 │ $250.00 $231.59 $18.41 $3,308.82 │ $200.00 $ 93.75 $106.25 $9,721.76 │ $450.00 $325.34 $124.66
// May 2025 │ $250.00 $232.79 $17.21 $3,076.03 │ $200.00 $ 94.76 $105.24 $9,627.00 │ $450.00 $327.55 $122.45
// Jun 2025 │ $250.00 $234.00 $16.00 $2,842.03 │ $200.00 $ 95.79 $104.21 $9,531.21 │ $450.00 $329.79 $120.21
// Jul 2025 │ $250.00 $235.22 $14.78 $2,606.81 │ $200.00 $ 96.82 $103.18 $9,434.39 │ $450.00 $332.04 $117.96
// Aug 2025 │ $250.00 $236.44 $13.56 $2,370.37 │ $200.00 $ 97.87 $102.13 $9,336.52 │ $450.00 $334.31 $115.69
// Sep 2025 │ $250.00 $237.67 $12.33 $2,132.70 │ $200.00 $ 98.93 $101.07 $9,237.59 │ $450.00 $336.60 $113.40
// Oct 2025 │ $250.00 $238.91 $11.09 $1,893.79 │ $200.00 $100.00 $100.00 $9,137.59 │ $450.00 $338.91 $111.09
// Nov 2025 │ $250.00 $240.15 $ 9.85 $1,653.64 │ $200.00 $101.09 $ 98.91 $9,036.50 │ $450.00 $341.24 $108.76
// Dec 2025 │ $250.00 $241.40 $ 8.60 $1,412.24 │ $200.00 $102.18 $ 97.82 $8,934.32 │ $450.00 $343.58 $106.42
// Jan 2026 │ $250.00 $242.66 $ 7.34 $1,169.58 │ $200.00 $103.29 $ 96.71 $8,831.03 │ $450.00 $345.95 $104.05
// Feb 2026 │ $250.00 $243.92 $ 6.08 $  925.66 │ $200.00 $104.40 $ 95.60 $8,726.63 │ $450.00 $348.32 $101.68
// Mar 2026 │ $250.00 $245.19 $ 4.81 $  680.47 │ $200.00 $105.53 $ 94.47 $8,621.10 │ $450.00 $350.72 $ 99.28
// Apr 2026 │ $250.00 $246.46 $ 3.54 $  434.01 │ $200.00 $106.68 $ 93.32 $8,514.42 │ $450.00 $353.14 $ 96.86
// May 2026 │ $250.00 $247.74 $ 2.26 $  186.27 │ $200.00 $107.83 $ 92.17 $8,406.59 │ $450.00 $355.57 $ 94.43
// Jun 2026 │ $187.24 $186.27 $ 0.97 $    0.00 │ $262.76 $171.76 $ 91.00 $8,234.83 │ $450.00 $358.03 $ 91.97
// Jul 2026 │                                  │ $450.00 $360.86 $ 89.14 $7,873.97 │ $450.00 $360.86 $ 89.14
// Aug 2026 │                                  │ $450.00 $364.76 $ 85.24 $7,509.21 │ $450.00 $364.76 $ 85.24
// Sep 2026 │                                  │ $450.00 $368.71 $ 81.29 $7,140.50 │ $450.00 $368.71 $ 81.29
// Oct 2026 │                                  │ $450.00 $372.70 $ 77.30 $6,767.80 │ $450.00 $372.70 $ 77.30
// Nov 2026 │                                  │ $450.00 $376.74 $ 73.26 $6,391.06 │ $450.00 $376.74 $ 73.26
// Dec 2026 │                                  │ $450.00 $380.82 $ 69.18 $6,010.24 │ $450.00 $380.82 $ 69.18
// Jan 2027 │                                  │ $450.00 $384.94 $ 65.06 $5,625.30 │ $450.00 $384.94 $ 65.06
// Feb 2027 │                                  │ $450.00 $389.11 $ 60.89 $5,236.19 │ $450.00 $389.11 $ 60.89
// Mar 2027 │                                  │ $450.00 $393.32 $ 56.68 $4,842.87 │ $450.00 $393.32 $ 56.68
// Apr 2027 │                                  │ $450.00 $397.58 $ 52.42 $4,445.29 │ $450.00 $397.58 $ 52.42
// May 2027 │                                  │ $450.00 $401.88 $ 48.12 $4,043.41 │ $450.00 $401.88 $ 48.12
// Jun 2027 │                                  │ $450.00 $406.23 $ 43.77 $3,637.18 │ $450.00 $406.23 $ 43.77
// Jul 2027 │                                  │ $450.00 $410.63 $ 39.37 $3,226.55 │ $450.00 $410.63 $ 39.37
// Aug 2027 │                                  │ $450.00 $415.07 $ 34.93 $2,811.48 │ $450.00 $415.07 $ 34.93
// Sep 2027 │                                  │ $450.00 $419.57 $ 30.43 $2,391.91 │ $450.00 $419.57 $ 30.43
// Oct 2027 │                                  │ $450.00 $424.11 $ 25.89 $1,967.80 │ $450.00 $424.11 $ 25.89
// Nov 2027 │                                  │ $450.00 $428.70 $ 21.30 $1,539.10 │ $450.00 $428.70 $ 21.30
// Dec 2027 │                                  │ $450.00 $433.34 $ 16.66 $1,105.76 │ $450.00 $433.34 $ 16.66
// Jan 2028 │                                  │ $450.00 $438.03 $ 11.97 $  667.73 │ $450.00 $438.03 $ 11.97
// Feb 2028 │                                  │ $450.00 $442.77 $  7.23 $  224.96 │ $450.00 $442.77 $  7.23
// Mar 2028 │                                  │ $227.40 $224.96 $  2.44 $    0.00 │ $227.40 $224.96 $  2.44

var planningItems = new List<BudgetPlanningItem>
{
    new BudgetPlanningItem { Name = "Paycheck", Amount = 3000m, Recurrence = "w-2"},
    new BudgetPlanningItem { Name = "Retirement Pension", Amount = 900m, Recurrence = "last"},
    new BudgetPlanningItem { Name = "Grocery allowance ($400/mo)", Amount = -200m, Recurrence = "1,15"},
    ...
};
var budget = Budget.GenerateSimpleBudget(planningItems);
TextGrid.FromCollection(budget).Render();

//    Date Name                               Amount Running Total
//  ------ -----------------------------  ---------- -------------
//   Apr 1 Bill Payment #1                   $30.00     $1,970.00
//   Apr 1 Grocery allowance ($400/mo)      $200.00     $1,770.00
//   Apr 1 Eating out allowance ($150/mo)    $75.00     $1,695.00
//   Apr 1 Insurance                        $200.00     $1,495.00
//   Apr 1 Mortgage                       $1,000.00       $495.00
//   Apr 1 Gas allowance ($300/mo)          $150.00       $345.00
//   Apr 1 Bill Payment #5                   $30.00       $315.00
//   Apr 1 Amazon allowance ($60/mo)         $30.00       $285.00
//   Apr 2 Bill Payment #2                   $60.00       $225.00
//   Apr 2 Bill Payment #6                   $60.00       $165.00
//   Apr 3 Bill Payment #7                   $90.00        $75.00
//   Apr 3 Bill Payment #3                   $90.00        $15.00-
//   Apr 4 Paycheck                       $3,000.00+    $2,985.00
//   Apr 4 Tithing                          $300.00     $2,685.00
//   Apr 4 Bill Payment #8                  $120.00     $2,565.00
//   Apr 4 Bill Payment #4                  $120.00     $2,445.00
//  Apr 15 Grocery allowance ($400/mo)      $200.00     $2,245.00
//  Apr 15 Eating out allowance ($150/mo)    $75.00     $2,170.00
//  Apr 15 Amazon allowance ($60/mo)         $30.00     $2,140.00
//  Apr 15 Gas allowance ($300/mo)          $150.00     $1,990.00
//  Apr 18 Paycheck                       $3,000.00+    $4,990.00
//  Apr 18 Tithing                          $300.00     $4,690.00
//  Apr 30 Retirement Pension               $900.00+    $5,590.00
```

#### Horseshoe.NET.Http

```c#
// basic HTTP call
var apiResponse = Get.AsJson<WebServiceResponse<string>>
(
    "https://site.com/service/endpoint"
);
apiResponse.Data;    // { "requestedItems" : [ { "name": "Item ABC"}, { "name": "Item DEF"}... ] }

// HTTP call with JWT authorization
var token = "eyjg73ls0...";
var apiResponse = Get.AsJson<WebServiceResponse<string>>
(
    "https://site.com/service/endpoint", 
    alterHeaders: (hdrs) =>
        hdrs.Add(HttpRequestHeader.Authorization, "Bearer " + token)
);
apiResponse.Data;    // { "authorizedItems" : [ { "name": "Item ABC"}, { "name": "Item DEF"}... ] }
```

#### Horseshoe.NET.Jwt

```c#
// mint a token
// note: the same digital signature crypto key that creates the digital signature is also used to validate it
TokenService.CreateAccessToken 
(
    tokenKey,            // e.g. encoding.GetBytes("ah476&ewj^!09")
    roles,               // e.g. { "All Contractors", "Domain Admin" }
    keyId,               // e.g. "0001"
    lifespanInSeconds,   // default is 3600 (1 hour)
    securityAlgorithm    // default is "HS256"
);                                            // "eyjg73ls0..." (encoded JWT)

// parse a token
// note: If ADFS '/token' provided the JWT roles will include Active Directory group memberships
TokenService.ParseToken("eyjg73ls0...");      // -> token as instance of AccessToken
```

#### Horseshoe.NET.OleDb|Odbc|OracleDb|SqlDb

```c#
// DB connection not established and statement not executed yet
var query = Query.FromStatement
(
    "Server=MYSVR;Database=MYDB;UID=me;PWD=myPwd"                        // (ODBC)
    -or-
    "Data Source=me/myPwd@//MYSVR:1650//MYDB"                            // (Oracle)
    -or-
    "Data Source=MYSVR;Initial Catalog=MYDB;User ID=me;Password=myPwd",  // (SQL Server)
    "SELECT * FROM [MySchema].[MyTable]"
); 

// creating a row parser offers fine grained control over converting data rows to objects
var rowParser = new RowParser<MyModel>
(
    (IDataReader reader) => new MyModel
    {
        MyStringProperty = Zap.String(reader["varcharColumn"]),
        MyNonNullStringProperty = (string)reader["nonNullVarcharColumn"],
        MyNullableIntProperty = Zap.NInt(reader["intColumn"]),
        MyNonNullIntProperty = (int)reader["nonNullIntColumn"],
        MyCustomProperty = new MyCustomClass
        {
            ID = ((int)reader["nonNullIntColumn2"], 
            Description = (string)reader["nonNullVarcharColumn2"]
        }
    }
);

// the following causes the DB connection to open and the statement to execute
IEnumerable<MyModel> list = query.AsList(rowParser);
string str = Zap.String(query.AsScalar());
DataTable dataTable = query.AsDataTable("My DataTable");
```

#### Horseshoe.NET.SecureIO

```c#
pseudoConnectionString = "sftp://username@11.22.33.44//root/subdir?password=PA$$w0rd";

var dirContents = Sftp.ListDirectoryContents
(
    fileMask: FtpFileMasks.Txt,           // or "*.txt"
    connectionInfo: pseudoConnectionString
);
foreach (var fileName in dirContents)
    Console.WriteLine(fileName);

Sftp.CreateFile
(
    "hello.txt",                          // remote dest file name
    "Hello World!",                       // file contents
    connectionInfo: pseudoConnectionString
);

Sftp.UploadFile
(
    "C:\\hello.txt",                      // local src file path
    connectionInfo: pseudoConnectionString
);

var stream = Sftp.DownloadFile
(
    "hello.txt",                          // remote src file name
    connectionInfo: pseudoConnectionString
);
Console.WriteLine("File length: " + stream.Length);
Console.WriteLine("File contents: " + encoding.GetString(stream.ToArray()));

Sftp.DeleteFile
(
    "hello.txt",                          // remote src file name
    connectionInfo: pseudoConnectionString
);

```

#### Horseshoe.NET.Text

```c#
var phrase = "Å¢t Øñę\u0000”;
TextClean.ToAsciiPrintable(phrase);  // -> "Act One" (Unicode -> ASCII, [NUL] -> "")
TextUtil.Reveal(phrase, options: RevealOptions.All);
// "Å¢t Øñę" -> ['Å'-197]['¢'-162]['t'-116][space]['Ø'-216]['ñ'-241]['ę'-281][NUL]
// "Act One" -> [‘A’-65 ][‘c’-99 ][’t’-116][space]['O'-79 ][’n'-110]['e'-101]
```