using cts.core.svc.contracts;
using cts.core.svc.domain;

namespace cts.core.svc.application.Abstractions.Authentication;

public interface IAccessTokenGenerator
{
    AccessToken Generate(User user);
}