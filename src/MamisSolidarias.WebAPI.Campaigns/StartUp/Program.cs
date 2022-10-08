using MamisSolidarias.WebAPI.Campaigns.StartUp;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
ServiceRegistrator.Register(builder);

var app = builder.Build();
MiddlewareRegistrator.Register(app);

app.Run();