namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// Defines all properties and methods common to Horseshoe.NET criterinators.
    /// </summary>
    /// <typeparam name="T">A comparable type.</typeparam>
    public interface ICriterinator<T>
    {
        /// <summary>
        /// Indicates whether the input item is a criteria match.
        /// </summary>
        /// <param name="input">An instance of <c>T</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        bool IsMatch(T input);
    }
}
