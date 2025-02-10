using System.Text.Json.Serialization;
using SkillSwap.Server.Configurations;
using SkillSwap.Server.Constants;
using SkillSwap.Server.Middlewares;
using SkillSwap.Services;
using SkillSwap.Services.Repositories;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddCustomApiSecurity(configuration);
builder.Services.AddCustomServiceDependencyRegister(configuration);
builder.Services.AddCustomDatabaseConfiguration(configuration);

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ISkillsRepository, SkillsRepository>();
builder.Services.AddScoped<IUserSkillsRepository, UserSkillsRepository>();
builder.Services.AddScoped<IMentorshipRequestsRepository, MentorshipRequestsRepository>();
builder.Services.AddScoped<ISessionsRepository, SessionsRepository>();
builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();

builder.Services.AddScoped<IEmailPasswordResets, EmailPasswordResetsService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<SkillsService>();
builder.Services.AddScoped<UserSkillsService>();
builder.Services.AddScoped<MentorshipRequestsService>();
builder.Services.AddScoped<SessionsService>();
builder.Services.AddScoped<ReviewsService>();
builder.Services.AddScoped<PaymentsService>();

builder.Services.AddAuthorizationBuilder()
                .AddPolicy(ApiConstants.PolicyUser, policy => policy
                .RequireRole(ApiConstants.PolicyUser));

builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiConstants.AllowLocalhost,
        builder =>
        {
            builder.WithOrigins(ApiConstants.OriginReact)
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors(ApiConstants.AllowLocalhost);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
