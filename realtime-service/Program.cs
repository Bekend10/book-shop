using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using realtime_service.Consumers;
using realtime_service.Data;
using realtime_service.Helpers.AuthHelpers;
using realtime_service.Hubs;
using realtime_service.Mapper;
using realtime_service.Providers;
using realtime_service.Repositories;
using realtime_service.Repositories.Implements;
using realtime_service.Repositories.Interfaces;
using realtime_service.Services.External;
using realtime_service.Services.Implements;
using realtime_service.Services.Interfaces;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PublishConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<NotificationConsumer>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddSingleton<IUserIdProvider, QueryStringUserIdProvider>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MessageMapper));
builder.Services.AddRefitClient<IUserService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("https://bookshop-api-fzcufhccfag3cgds.canadacentral-01.azurewebsites.net/api/v1");
    })
    .AddHttpMessageHandler<AuthHeaderHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notification");  

app.Run();
