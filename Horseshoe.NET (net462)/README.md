![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-256x256.png)

# Horseshoe.NET

A suite of .NET utilities with a dual purpose: simplify coding tasks and offer a variety of unique capabilities for developers. What this represents is a career's worth of collecting useful code snippets packaged as a NuGet library that any .NET developer can utilize.  

A large portion of this code base is dedicated to replacing verbose, repetitive boilerplate code for tasks such as configuration, ADO.NET / ODP.NET database queries and web API calls with highly customizable single line code replacements.

## Code Examples

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

#### Horseshoe.NET.DataImport

```c#
// [villains.csv]
// Name,Age,Location
// "Snape, Severus",47,Hogwarts
// Maleficent,39,Enchanted Castle
// Jafar,59,Cave of Wonders

var options = new DataImportOptions { HasHeaderRow = true };
TabularData.ImportCommaDelimitedText("villains.csv", options: options);
```


#### Horseshoe.NET.Text

```c#
var phrase = "Å¢t Øñę\u0000”;
TextClean.ToAsciiPrintable(phrase);  // -> "Act One" (Unicode -> ASCII, [NUL] -> "")
TextUtil.Reveal(phrase, options: RevealOptions.All);
// "Å¢t Øñę" -> ['Å'-197]['¢'-162]['t'-116][space]['Ø'-216]['ñ'-241]['ę'-281][NUL]
// "Act One" -> [‘A’-65 ][‘c’-99 ][’t’-116][space]['O'-79 ][’n'-110]['e'-101]
```