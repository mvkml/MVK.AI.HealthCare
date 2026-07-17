using HC.AI.MAPI.AL;
using HC.AI.MAPI.BL.HelloWorld;
using HC.AI.MAPI.Llm;
using HC.AI.MAPI.Tool;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));
builder.Services.AddHttpClient<IOllamaClient, OllamaClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddHttpClient<PatientApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PatientApi:BaseUrl"] ?? "http://localhost:5295");
});

builder.Services.AddScoped<HealthcareQueryTool>();
builder.Services.AddScoped<AgentV0>();
builder.Services.AddScoped<IHelloWorldBL, HelloWorldBL>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
