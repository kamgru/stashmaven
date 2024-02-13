using System.Text.Json.Serialization;
using StashMaven.WebApi;
using StashMaven.WebApi.Data.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddFeatures();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<CacheReader>();
builder.Services.AddWebApiAuthentication(builder.Configuration);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("allow-all", policyBuilder =>
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        AadConfig aadConfig = builder.Configuration.GetAadConfiguration();
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "StashMaven.WebApi v1");
        opt.OAuthClientId(aadConfig.ClientId);
        opt.OAuthUsePkce();
        opt.OAuthScopeSeparator(" ");
        opt.ShowExtensions();
    });
    app.UseCors("allow-all");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
