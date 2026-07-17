using HC.AI.MAPI.AL;
using HC.AI.MAPI.BL.HelloWorld;
using HC.AI.MAPI.Llm;
using HC.AI.MAPI.Prompt.Doctor;
using HC.AI.MAPI.Services;
using HC.AI.MAPI.Tool;
using HC.AI.MAPI.Tool.Validation;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

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
builder.Services.AddScoped<IDoctorAgent, DoctorAgent>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IDoctorPromptProvider, DoctorPromptProvider>();
builder.Services.AddScoped<IPromptValidationUtility, PromptValidationUtility>();

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
