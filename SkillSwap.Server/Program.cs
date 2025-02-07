using SkillSwap.Server.Configurations;
using SkillSwap.Server.Constants;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddCustomDatabaseConfiguration(configuration);

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

app.UseCors(ApiConstants.AllowLocalhost);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
