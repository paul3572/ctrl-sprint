namespace cts.core.svc.application.Auth.Exceptions;

public sealed class EmailAlreadyRegisteredException : Exception
{
    public EmailAlreadyRegisteredException()
        : base("A user with this email address already exists.")
    {
    }
}