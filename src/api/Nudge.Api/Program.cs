using Nudge.Learning.Extensions;
using Nudge.Shared.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructure();

var app = builder.Build();

app.UseInfrastructure();

app.Run();
