using Microsoft.Extensions.Options;
using Nudge.Bot;
using Nudge.Bot.Commands;
using Nudge.Bot.Localization;
using Nudge.Bot.NudgeApi;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<TelegramOptions>()
    .Bind(builder.Configuration.GetSection(TelegramOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<NudgeApiOptions>()
    .Bind(builder.Configuration.GetSection(NudgeApiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var token = sp.GetRequiredService<IOptions<TelegramOptions>>().Value.BotToken;
        return new TelegramBotClient(token, httpClient);
    });

builder.Services.AddHttpClient<IUserApiClient, UserApiClient>((sp, httpClient) =>
{
    var baseUrl = sp.GetRequiredService<IOptions<NudgeApiOptions>>().Value.BaseUrl;
    httpClient.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddSingleton<IBotMessageResolver, BotMessageResolver>();

// Singleton, not scoped: TelegramPollingService (a singleton BackgroundService) injects
// IEnumerable<IBotCommandHandler> directly, so a scoped handler would be a captive dependency.
builder.Services.AddSingleton<IBotCommandHandler, StartCommandHandler>();

builder.Services.AddHostedService<TelegramPollingService>();

var host = builder.Build();
host.Run();
