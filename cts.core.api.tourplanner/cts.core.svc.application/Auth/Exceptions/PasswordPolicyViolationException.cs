namespace cts.core.svc.application.Auth.Exceptions;

public sealed class PasswordPolicyViolationException : Exception
{
    public PasswordPolicyViolationException(string message)
        : base(message)
    {
    }
}