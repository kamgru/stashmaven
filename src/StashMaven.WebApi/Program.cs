using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;
using StashMaven.WebApi.PartnerFeatures;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StashMavenContext>(opt =>
{
    string writeModelConnectionString = builder.Configuration.GetConnectionString("WriteModel")
                                        ?? throw new InvalidOperationException(
                                            "WriteModel connection string is missing.");
    opt.UseNpgsql(writeModelConnectionString);
});
builder.Services.AddScoped<CreatePartnerHandler>();
builder.Services.AddScoped<GetPartnerByIdHandler>();
builder.Services.AddScoped<ListPartnersHandler>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("allow-all", policyBuilder =>
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("allow-all");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
