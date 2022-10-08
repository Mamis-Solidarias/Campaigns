using MamisSolidarias.WebAPI.Campaigns.StartUp;

var builder = WebApplication.CreateBuilder(args);
ServiceRegistrator.Register(builder);

var app = builder.Build();
MiddlewareRegistrator.Register(app);

app.Run();