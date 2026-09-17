using System.ComponentModel.DataAnnotations;

namespace Nudge.Bot.NudgeApi;

public class NudgeApiOptions
{
    public const string SectionName = "NudgeApi";

    [Required]
    public string BaseUrl { get; set; } = string.Empty;
}
