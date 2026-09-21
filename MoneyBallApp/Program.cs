using Microsoft.ML.OnnxRuntime;
using MoneyBallApp.Components;
using MoneyBallApp.Components.States;
using MoneyBallApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services
    .AddSingleton<KeeperParser>()
    .AddSingleton<CentreBackParser>()
    .AddSingleton<WingFullBackParser>();


// Load ONNX models
builder.Services.AddKeyedSingleton<InferenceSession>(
    "goalkeeper",
    new InferenceSession("MachineLearning/goalkeeperXGBoost.onnx")
);

builder.Services.AddKeyedSingleton<InferenceSession>(
    "centreback",
    new InferenceSession("MachineLearning/centrebackXGBoost.onnx")
);

builder.Services.AddKeyedSingleton<InferenceSession>(
    "wingfullback",
    new InferenceSession("MachineLearning/wingfullbackXGBoost.onnx")
);

builder.Services.AddScoped<KeeperPredictorService>();
builder.Services.AddScoped<KeeperState>();
builder.Services.AddScoped<CBState>();
builder.Services.AddScoped<CBPredictorService>();
builder.Services.AddScoped<WFBState>();
builder.Services.AddScoped<WFBPredictorService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
