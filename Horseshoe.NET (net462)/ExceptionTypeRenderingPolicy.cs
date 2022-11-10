namespace Horseshoe.NET
{
    /// <summary>
    /// A basic <c>Exception</c> descriptor class suitable for JSON serialization
    /// </summary>
    public enum ExceptionTypeRenderingPolicy
    {
        Fqn,
        FqnExceptSystem,
        NameOnly
    }
}
