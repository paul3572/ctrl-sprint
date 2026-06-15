using cts.core.svc.contracts;

namespace cts.core.svc.application.Abstractions.Authentication;

public interface IAccessTokenGenerator
{
    AccessToken Generate(User user);
}