using Nudge.Shared.EFCore;

namespace Nudge.Identity.Data;

public class IdentityDbOptions : IConnectionStringOptions
{
    public const string SectionName = "IdentityDb";

    public string ConnectionString { get; set; } = string.Empty;
}
