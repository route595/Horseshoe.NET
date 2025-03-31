namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Speficies whether to render a row / column position as R1:C1 or A1 (e.g. Excel)
    /// </summary>
    public enum PositionNotation
    {
        /// <summary>
        /// The default reference formatting style (e.g. R109:C7)
        /// </summary>
        R1C1,

        /// <summary>
        /// An Excel-type reference formatting style (e.g. G109)
        /// </summary>
        A1
    }
}
