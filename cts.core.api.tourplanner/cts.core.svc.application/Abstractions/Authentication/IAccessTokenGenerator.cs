using cts.core.svc.contracts.Users;

namespace cts.core.svc.application.Abstractions.Authentication;

public interface IAccessTokenGenerator
{
    AccessToken Generate(User user);
}