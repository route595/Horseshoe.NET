using System;
using System.Data;
using System.Data.Common;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// <para>
    /// A flexible, generic <c>DbParameter</c> that can be used on any database query in Horseshoe.NET
    /// because it converts parameters to the type when a statement is created and executed.  
    /// </para>
    /// <para>
    /// One common pain point that it fixes is the null value problem.  Sometimes, parameters with 
    /// null values are ignored, but not if you set the value to <c>DbNull.Value</c>.  It's a small
    /// win but setting the value to null actually sets it to <c>DbNull.Value</c> for you.
    /// </para>
    /// </summary>
    public class Parameter : DbParameter
    {
        private object _value;

        private DbType? _dbType;

        /// <summary>
        /// The parameter name
        /// </summary>
        public override string ParameterName { get; set; }

        /// <summary>
        /// The parameter value
        /// </summary>
        public override object Value
        {
            get { return _value ?? DBNull.Value; }
            set { _value = DbUtil.WrangleParameterValue(value); }
        }

        /// <summary>
        /// The DB data type
        /// </summary>
        public override DbType DbType
        {
            get { return _dbType ?? DbUtil.CalculateDbType(Value); }
            set { _dbType = value; }
        }

        /// <summary>
        /// Whether the DB data type has been set
        /// </summary>
        public bool IsDbTypeSet => _dbType.HasValue;

        /// <summary>
        /// Parameter direction (default is <c>Input</c>)
        /// </summary>
        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        /// <summary>
        /// Whether the source column that aligns with this parameter allows <c>null</c>
        /// </summary>
        public override bool IsNullable { get; set; }

        /// <summary>
        /// The size of the source column if numeric, text or binary
        /// </summary>
        public override int Size { get; set; }

        /// <summary>
        /// The name of the source column
        /// </summary>
        public override string SourceColumn { get; set; }

        /// <summary>
        /// Source column null mapping
        /// </summary>
        public override bool SourceColumnNullMapping { get; set; }

        /// <summary>
        /// <c>DataRow</c> version
        /// </summary>
        public override DataRowVersion SourceVersion { get; set; } = DataRowVersion.Default;

        /// <summary>
        /// Creates a new <c>Parameter</c>
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Creates a new <c>Parameter</c>
        /// </summary>
        /// <param name="parameterName">parameter name</param>
        /// <param name="value">parameter value</param>
        public Parameter(string parameterName, object value)
        {
            if (parameterName != null)
                ParameterName = parameterName;
            Value = value;
        }

        /// <summary>
        /// Reset DB data type
        /// </summary>
        public override void ResetDbType()
        {
            DbType = default;
        }
    }
}
