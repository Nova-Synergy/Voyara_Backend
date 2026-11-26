using Voyara.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<GuideRequestExpirationWorker>();

var host = builder.Build();
host.Run();
