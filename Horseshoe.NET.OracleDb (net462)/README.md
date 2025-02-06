![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.OracleDb

ODP.NET-based Oracle data access wrappers

## Code examples

```
// DB connection not established and statement not executed yet
var query = Query.FromStatement
(
	"Data Source=me/myPwd@//MYSVR:1650//MYDB", 
	"SELECT * FROM MyTable"
); 

// creating a row parser offers fine grained control over converting data rows to objects
var rowParser = new RowParser<MyModel>
(
	(IDataReader reader) => new MyModel
	{
		MyStringProperty = Zap.String(reader["col1"]),
		MyNonNullStringProperty = (string)reader["col2"],
		MyNullableIntProperty = Zap.NInt(reader["col3"]),
		MyNonNullIntProperty = (int)reader["col4"],
		MyCompoundProperty = new MyOtherClass((int)reader["col5"], (string)reader["col6"])
	}
);

// the following will cause the DB connection to open and the statement to execute
List<MyModel> list = query.AsCollection(rowParser);
string str = Zap.String(query.AsScalar());
DataTable dataTable = query.AsDataTable();
```
