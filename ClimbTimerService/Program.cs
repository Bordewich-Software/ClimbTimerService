using ClimbTimerService.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.WebSockets;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSingleton<TimerService>();
services.AddHostedService<PeriodicEventPublisher>();

services.AddCors(s =>
{
    s.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

services.AddWebSockets(c => {
    c.AllowedOrigins.Add("http://localhost:3000");
    c.AllowedOrigins.Add("http://localhost");
});

services.AddGraphQLServer()
    .AddQueryType()
    .AddSubscriptionType()
    .AddMutationType()
    .AddMutationConventions()
    .RegisterService<TimerService>()
    .AddClimbTimerTypes()
    .AddInMemorySubscriptions();

var app = builder.Build();

app.UseCors();

app.UseWebSockets();

app.MapGraphQL();
app.MapGraphQLWebSocket();

app.Run();
