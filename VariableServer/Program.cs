using System.Reflection;
using System.Text.Json;
using IctBaden.RevolutionPi;
using IctBaden.RevolutionPi.Configuration;
using VariableServer.Model;

var builder = WebApplication.CreateBuilder(args);

// Default to the legacy port unless overridden via --urls / ASPNETCORE_URLS.
if (string.IsNullOrEmpty(builder.Configuration["urls"]) &&
    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://*:8000");
}

builder.Services.AddResponseCompression();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Single device, opened once at startup (this is an on-device debug tool).
// Tolerate a missing driver/config so the server still starts off-device.
var control = new PiControl();
var config = new PiConfiguration();
try
{
    control.Open();
    config.Open();
}
catch (DllNotFoundException ex)
{
    Console.Error.WriteLine($"piControl driver not available (running off-device?): {ex.Message}");
}
builder.Services.AddSingleton(control);
builder.Services.AddSingleton(config);

var json = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

var app = builder.Build();
app.UseResponseCompression();
app.UseCors();

app.MapGet("/", () => Results.Json(new
{
    service = "RevolutionPi Variable Server",
    version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
    varlist = "GET ~/variables",
    readvar = "GET ~/variables/{varname}",
    readprop = "GET ~/variables/{varname}/{propname}"
}, json));

app.MapGet("/variables", (PiConfiguration cfg) =>
{
    var varlist = cfg.Devices
        .SelectMany(d => d.Variables)
        .Select(v => new
        {
            name = v.Name,
            type = v.Type.ToString(),
            length = v.LengthText
        });
    return Results.Json(varlist, json);
});

app.MapGet("/variables/{varname}", (string varname, PiConfiguration cfg, PiControl ctl) =>
{
    var (info, error) = ReadVariable(cfg, ctl, varname);
    return error ?? Results.Json(info, json);
});

app.MapGet("/variables/{varname}/{propname}", (string varname, string propname, PiConfiguration cfg, PiControl ctl) =>
{
    var (info, error) = ReadVariable(cfg, ctl, varname);
    if (error != null) return error;

    var prop = typeof(VarReadInfo).GetProperties()
        .FirstOrDefault(p => string.Equals(p.Name, propname, StringComparison.InvariantCultureIgnoreCase));
    if (prop == null)
    {
        return Results.Json(new { error = "Unknown property", name = propname }, json,
            statusCode: StatusCodes.Status404NotFound);
    }

    return Results.Json(prop.GetValue(info), json);
});

app.Run();

// Reads a variable by name, returning either the populated info or an error result.
(VarReadInfo Info, IResult Error) ReadVariable(PiConfiguration cfg, PiControl ctl, string varname)
{
    var varInfo = cfg.Devices
        .SelectMany(d => d.Variables)
        .FirstOrDefault(v => string.Equals(v.Name, varname, StringComparison.InvariantCultureIgnoreCase));

    if (varInfo == null)
    {
        return (null, Results.Json(new { error = "Variable not found", name = varname }, json,
            statusCode: StatusCodes.Status404NotFound));
    }

    var varData = ctl.ReadVariable(varInfo);
    if (varData == null)
    {
        return (null, Results.Json(new { error = "Could not read variable", name = varname }, json,
            statusCode: StatusCodes.Status503ServiceUnavailable));
    }

    var info = new VarReadInfo
    {
        Name = varInfo.Name,
        DefaultValue = varInfo.DefaultValue,
        Comment = varInfo.Comment,
        Type = varInfo.Type.ToString(),
        LengthText = varInfo.LengthText,
        Length = varInfo.Length,
        Address = varInfo.Address,
        Device = new VarDeviceInfo
        {
            Name = varInfo.Device.Name,
            Offset = varInfo.Device.Offset
        },
        Data = varData.Raw.Select(d => (int)d).ToArray(),
        Value = varData.Value
    };
    return (info, null);
}
