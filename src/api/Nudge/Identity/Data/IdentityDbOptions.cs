namespace Nudge.Identity.Data;

public class IdentityDbOptions
{
    public const string SectionName = "IdentityDb";

    public string ConnectionString { get; set; } = string.Empty;
}
