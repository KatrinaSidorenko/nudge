using System.ComponentModel.DataAnnotations;

namespace Nudge.Bot.NudgeApi;

// Kept separate from NudgeApiOptions (which only holds the host's BaseUrl): each Nudge.Api
// resource can evolve its own version independently, so each gets its own options type.
public class UsersApiOptions
{
    public const string SectionName = "NudgeApi:Users";

    [Required]
    public string Version { get; set; } = "1";
}
