using Nudge.Shared.EFCore;

namespace Nudge.Learning.Data;

public class LearningDbOptions : IConnectionStringOptions
{
    public const string SectionName = "LearningDb";

    public string ConnectionString { get; set; } = string.Empty;
}
