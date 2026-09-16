using Microsoft.Extensions.Options;
using Nudge.Bot;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<TelegramOptions>()
    .Bind(builder.Configuration.GetSection(TelegramOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var token = sp.GetRequiredService<IOptions<TelegramOptions>>().Value.BotToken;
        return new TelegramBotClient(token, httpClient);
    });

builder.Services.AddHostedService<TelegramPollingService>();

var host = builder.Build();
host.Run();
