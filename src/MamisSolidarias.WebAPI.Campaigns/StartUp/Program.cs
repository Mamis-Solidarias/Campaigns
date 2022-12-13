using MamisSolidarias.WebAPI.Campaigns.StartUp;

var builder = WebApplication.CreateBuilder(args);
ServiceRegistrar.Register(builder);

var app = builder.Build();
MiddlewareRegistrar.Register(app);

app.Run();