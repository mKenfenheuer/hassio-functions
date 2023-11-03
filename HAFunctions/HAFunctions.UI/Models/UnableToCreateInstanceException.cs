namespace HAFunctions.UI.Models;

[Serializable]
public class UnableToCreateInstanceException : Exception
{
    public Exception[] InnerExceptions { get; set; }
    public UnableToCreateInstanceException(Exception[] innerExceptions)
    {
        InnerExceptions = innerExceptions;
    }

    public UnableToCreateInstanceException(string? message, Exception[] innerExceptions = null) : base(message)
    {
        InnerExceptions = innerExceptions;
    }
}
