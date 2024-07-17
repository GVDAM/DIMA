using Dima.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

var cnnStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

builder.Services.AddDbContext<AppDbContext>(x =>
{
    //x.UseSqlServer(cnnStr);
    x.UseInMemoryDatabase(cnnStr);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
x.CustomSchemaIds(n => n.FullName);
});

builder.Services.AddTransient<Handler>();

var app = builder.Build();

app.UseSwagger(c => c.RouteTemplate = "swagger/index.html");
app.UseSwaggerUI(c => c.SwaggerEndpoint("v1.json", "Test API"));

app.MapGet("/", () => "Hello World!");

app.MapPost(
    "/v1/transactions",
    (Request request, Handler handler)
    => handler.Handle(request))
    .WithName("Transactions: Create")
    .WithSummary("Cria uma nova transação")
    .Produces<Response>();

app.Run();

public class Request
{
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public long CategoryId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public class Response
{
    public string Message { get; set; } = string.Empty;
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class Handler
{
    public Response Handle(Request request)
    {

        return new Response
        {
            Id = 4,
            Title = request.Title,
        };
    }
}

[JsonSerializable(typeof(Response[]))]
[JsonSerializable(typeof(Request[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}