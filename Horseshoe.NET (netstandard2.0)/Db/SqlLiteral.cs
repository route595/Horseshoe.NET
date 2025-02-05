using System;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A set of useful, provider-dependent SQL literals to use as parameter values, for example
    /// </summary>
    public class SqlLiteral : IComparable<SqlLiteral>
    {
        /// <summary>
        /// Creates a new <c>SqlLiteral</c>.
        /// </summary>
        /// <param name="expression">The literal expression to use.</param>
        /// <exception cref="ValidationException"></exception>
        public SqlLiteral(string expression) : this(() => expression ?? throw new ValidationException("Cannot build a SQL literal from null"))
        {
        }

        /// <summary>
        /// Creates a new <c>SqlLiteral</c>.
        /// </summary>
        /// <param name="renderer">Renders the <c>SqlLiteral</c> as a <c>string</c> SQL expression.</param>
        /// <exception cref="ValidationException"></exception>
        public SqlLiteral(Func<string> renderer)
        {
            Renderer = renderer ?? throw new ValidationException("Cannot build a SQL literal with no renderer.");
        }

        /// <summary>
        /// Renders the <c>SqlLiteral</c> as a SQL expression.
        /// </summary>
        /// <returns>A SQL expression.</returns>
        public Func<string> Renderer { get; }

        /// <summary>
        /// Renders the <c>SqlLiteral</c> as a SQL expression.
        /// </summary>
        /// <returns>A SQL expression.</returns>
        public string Render()
        {
            return Renderer?.Invoke() ?? TextConstants.Null;
        }

        /// <summary>
        /// Returns a <c>string</c> rendering of this <c>SqlLiteral</c> (e.g. <c>SqlLiteral</c> returns <c>Expression</c>, subclasses may apply logic to return a value).
        /// </summary>
        /// <returns>A <c>string</c>.</returns>
        public override string ToString()
        {
            return Render();
        }

        /// <inheritdoc />
        public int CompareTo(SqlLiteral other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SQL literal expression for getting the current date/time.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc. Required.</param>
        /// <param name="dotNetOverride">If <c>true</c>, uses the .NET runtime's current date/time instead, default is <c>false</c>.</param>
        /// <returns>A SQL literal expression for getting the current date/time.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static SqlLiteral CurrentDate(DbProvider provider, bool dotNetOverride = false)
        {
            return new SqlLiteral
            (
                () =>
                {
                    if (dotNetOverride)
                        return DbUtil.Sqlize(DateTime.Now, provider: provider);

                    switch (provider)
                    {
                        case DbProvider.SqlServer:
                            return "GETDATE()";
                        case DbProvider.Oracle:
                            return "SYSDATE";
                        case DbProvider.Neutral:
                            throw new ArgumentException("This method requires a non-neutral DB provider.");
                        default:
                            throw new NotImplementedException("This provider does not yet have an implementation of 'get current date': " + provider);
                    }
                }
            );
        }

        /// <summary>
        /// The SQL literal expression for generating a GUID.
        /// </summary>
        /// <param name="provider">A DB provider lends hints about how to render SQL expressions and statements. Required.</param>
        /// <param name="dotNetOverride">If <c>true</c>, uses the .NET runtime to generate the GUID, default is <c>false</c>.</param>
        /// <returns>A SQL literal expression for generating a GUID.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static SqlLiteral NewGuid(DbProvider provider, bool dotNetOverride = false)
        {
            return new SqlLiteral
            (
                () =>
                {
                    if (dotNetOverride)
                        return "'" + Guid.NewGuid() + "'";

                    switch (provider)
                    {
                        case DbProvider.SqlServer:
                            return "NEWID()";
                        case DbProvider.Oracle:
                            return "SYSGUID()";
                        case DbProvider.Neutral:
                            throw new ArgumentException("This method requires a non-neutral DB provider.");
                        default:
                            throw new NotImplementedException("This provider does not yet have an implementation of 'new GUID': " + provider);
                    }
                }
            );
        }

        /// <summary>
        /// The SQL literal expression for getting the integer auto-increment ID after inserting a row
        /// </summary>
        /// <param name="provider">A DB provider lends hints about how to render SQL expressions and statements. Required.</param>
        /// <returns>A SQL literal expression for getting the last inserted row ID.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static SqlLiteral Identity(DbProvider provider)
        {
            return new SqlLiteral
            (
                () =>
                {
                    switch (provider)
                    {
                        case DbProvider.SqlServer:
                            return "CONVERT(int, SCOPE_IDENTITY())";
                        case DbProvider.Oracle:
                            return "LAST_INSERT_ID()";
                        case DbProvider.Neutral:
                            throw new ArgumentException("This method requires a non-neutral DB provider.");
                        default:
                            throw new NotImplementedException("This provider does not yet have an implementation of 'get identity': " + provider);
                    }
                }
            );
        }
    }
}
