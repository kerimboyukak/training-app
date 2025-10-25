namespace Training.Mobile.Exceptions;

public class BackendHttpException : HttpRequestException
{
    public System.Net.HttpStatusCode HttpCode { get; }

    public BackendHttpException(System.Net.HttpStatusCode code) : this(code, default!, default!)
    {
    }

    public BackendHttpException(System.Net.HttpStatusCode code, string message) : this(code, message, default!)
    {
    }

    public BackendHttpException(System.Net.HttpStatusCode code, string message, Exception inner) : base(message, inner)
    {
        HttpCode = code;
    }
}