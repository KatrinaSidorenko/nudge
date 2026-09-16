using System.ComponentModel.DataAnnotations;

namespace Nudge.Bot;

public class TelegramOptions
{
    public const string SectionName = "Telegram";

    [Required]
    public string BotToken { get; set; } = string.Empty;
}
