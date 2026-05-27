using TourPlanner.Domain.Users;

namespace TourPlanner.Application.Abstractions.Authentication;

public interface IAccessTokenGenerator
{
    AccessToken Generate(User user);
}