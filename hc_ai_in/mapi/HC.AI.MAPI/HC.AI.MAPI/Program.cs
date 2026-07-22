using HC.AI.MAPI.AL;
using HC.AI.MAPI.BL.Factory;
using HC.AI.MAPI.BL.HelloWorld;
using HC.AI.MAPI.BL.LLMModel;
using HC.AI.MAPI.BL.Persona;
using HC.AI.MAPI.Llm;
using HC.AI.MAPI.Prompt.Doctor;
using HC.AI.MAPI.Prompt.Patient;
using HC.AI.MAPI.Semantic.Factory;
using HC.AI.MAPI.SemanticProcess;
using HC.AI.MAPI.Services;
using HC.AI.MAPI.Services.Mapping;
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
builder.Services.AddScoped<IKernalFactory, KernalFactory>();
builder.Services.AddScoped<ISemanticProcessService, SemanticProcessService>();
builder.Services.AddScoped<IDoctorSemanticProcess, DoctorSemanticProcess>();
builder.Services.AddScoped<IDoctorPromptMapper, DoctorPromptMapper>();
builder.Services.AddScoped<ILLMModelBL, LLMModelBL>();
builder.Services.AddScoped<ILLMOptionsFactory, LLMOptionsFactory>();

builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IPatientPromptProvider, PatientPromptProvider>();
builder.Services.AddScoped<IPatientSemanticProcess, PatientSemanticProcess>();
builder.Services.AddScoped<IPatientPromptMapper, PatientPromptMapper>();

// EPIC001 (PB032/TASK017) — mock resolution mechanism only, not wired into the live Doctor
// provide-prompt path. See PersonaModelResolutionController and BL/Persona/*.
builder.Services.AddSingleton<IPersonaLlmConfigProvider, PersonaLlmConfigMockProvider>();
builder.Services.AddScoped<IPersonaModelResolutionBL, PersonaModelResolutionBL>();

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
