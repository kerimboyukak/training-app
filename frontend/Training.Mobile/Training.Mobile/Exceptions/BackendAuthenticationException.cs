namespace Training.Mobile.Exceptions;

public class BackendAuthenticationException : Exception
{
    public string? Content { get; }

    public BackendAuthenticationException()
    {
    }

    public BackendAuthenticationException(string content)
    {
        Content = content;
    }
}