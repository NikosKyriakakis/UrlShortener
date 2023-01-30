using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Repository.Pattern.Generic;
using Repository.Pattern.MongoDb.Settings;
using Repository.Pattern.MongoDB;
using UrlShortener.Service.Models;

var builder = WebApplication.CreateBuilder(args);
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddMongo()
    .AddMongoRepository<Url>("urls");

builder.Services.AddControllers().AddNewtonsoftJson(s =>
{
    s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});

builder.Services.AddMvc(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.MapType<TimeSpan>(() => new OpenApiSchema
{
    Type = "string",
    Example = new OpenApiString("00:00:00")
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapFallback(async (IRepository<Url> repository, HttpContext ctx) =>
{
    var path = ctx.Request.Path.ToUriComponent().Remove(0, 5);
    var requestedUrl = await repository.GetAsync(x => x.ShortUrl == path);
    if (requestedUrl == null) return Results.BadRequest();

    return Results.Redirect(requestedUrl.LongUrl);
});

app.Run();
