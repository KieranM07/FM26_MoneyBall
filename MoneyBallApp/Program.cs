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
    .AddSingleton<WingFullBackParser>()
    .AddSingleton<DefMidParser>()
    .AddSingleton<MidParser>()
    .AddSingleton<WingParser>()
    .AddSingleton<STParser>();


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

builder.Services.AddKeyedSingleton<InferenceSession>(
    "defmid",
    new InferenceSession("MachineLearning/defmidXGBoost.onnx")
);

builder.Services.AddKeyedSingleton<InferenceSession>(
    "mid",
    new InferenceSession("MachineLearning/midXGBoost.onnx")
);
builder.Services.AddKeyedSingleton<InferenceSession>(
    "winger",
    new InferenceSession("MachineLearning/wingXGBoost.onnx")
);
builder.Services.AddKeyedSingleton<InferenceSession>(
    "striker",
    new InferenceSession("MachineLearning/strikerXGBoost.onnx")
);

builder.Services.AddScoped<KeeperPredictorService>();
builder.Services.AddScoped<KeeperState>();
builder.Services.AddScoped<CBState>();
builder.Services.AddScoped<CBPredictorService>();
builder.Services.AddScoped<WFBState>();
builder.Services.AddScoped<WFBPredictorService>();
builder.Services.AddScoped<DMState>();
builder.Services.AddScoped<DMPredictorService>();
builder.Services.AddScoped<MState>();
builder.Services.AddScoped<MPredictorService>();
builder.Services.AddScoped<WState>();
builder.Services.AddScoped<WPredictorService>();
builder.Services.AddScoped<STState>();
builder.Services.AddScoped<STPredictorService>();

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
