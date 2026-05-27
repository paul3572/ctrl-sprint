namespace TourPlanner.Application.Auth.Exceptions;

public sealed class EmailAlreadyRegisteredException : Exception
{
    public EmailAlreadyRegisteredException()
        : base("A user with this email address already exists.")
    {
    }
}