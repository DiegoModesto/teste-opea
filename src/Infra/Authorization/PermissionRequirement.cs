using Microsoft.AspNetCore.Authorization;

namespace Infra.Authorization;

internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
