![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.Odbc

ADO.NET-based ODBC data access wrappers

## Code Examples

```c#
// DB connection not established and statement not executed yet
var query = Query.FromStatement
(
    "Server=MYSVR;Database=MYDB;UID=me;PWD=myPwd", 
    "SELECT * FROM [MyTable]"
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
