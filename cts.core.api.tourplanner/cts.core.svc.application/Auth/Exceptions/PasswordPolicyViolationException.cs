namespace TourPlanner.Application.Auth.Exceptions;

public sealed class PasswordPolicyViolationException : Exception
{
    public PasswordPolicyViolationException(string message)
        : base(message)
    {
    }
}