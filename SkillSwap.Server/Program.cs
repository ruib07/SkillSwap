using SkillSwap.Server.Configurations;
using SkillSwap.Server.Constants;
using SkillSwap.Services;
using SkillSwap.Services.Interfaces;
using SkillSwap.Services.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddCustomApiSecurity(configuration);
builder.Services.AddCustomServiceDependencyRegister(configuration);
builder.Services.AddCustomDatabaseConfiguration(configuration);

builder.Services.AddScoped<IUsers, UsersService>();
builder.Services.AddScoped<IEmailPasswordResets, EmailPasswordResetsService>();
builder.Services.AddScoped<ISkills, SkillsService>();
builder.Services.AddScoped<IMentorshipRequests, MentorshipRequestsService>();
builder.Services.AddScoped<ISessions, SessionsService>();
builder.Services.AddScoped<IReviews, ReviewsService>();
builder.Services.AddScoped<IPayments, PaymentsService>();

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

app.UseExceptionHandler("/error");
app.UseCors(ApiConstants.AllowLocalhost);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
