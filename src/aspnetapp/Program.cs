using System.Reflection;
using aspnetapp.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddRouting(options =>
    options.LowercaseUrls = true);

//Add API versioning to application
builder.Services.AddApiVersioning(options =>
{
    //Reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    //NOTE: the specified format code will format the version as "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";

    //NOTE: this option is only necessary when versioning by url segment
    options.SubstituteApiVersionInUrl = true;
});

// Register the Swagger generator, defining 1 or more Swagger documents
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Sample API",
        Description = "A simple hybrid REST and gRPC service using ASP.NET Core 6.0",
        Contact = new OpenApiContact
        {
            Name = "Ben Chen",
            Url = new Uri("https://linkedin.com/in/bchen04/")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://github.com/bchen04/aspnetcore-grpc-rest/blob/main/LICENSE")
        }
    });

    options.DescribeAllParametersInCamelCase();

    // Set the comments path for the Swagger JSON and UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Registers the service with a scoped lifetime
builder.Services.AddScoped<IGreeterService, GreeterService>();

// Enables gRPC
builder.Services.AddGrpc();

builder.WebHost.UseKestrel(options =>
{
    var services = options.ApplicationServices;

    // Use HTTP/1.x
    options.ListenAnyIP(4999, listenOptions =>
        listenOptions.Protocols = HttpProtocols.Http1);


    // Use HTTP/2
    options.ListenAnyIP(5000, listenOptions =>
        listenOptions.Protocols = HttpProtocols.Http2);


    //NOTE: This requires a platform that support QUIC or HTTP/3
    options.ListenAnyIP(5001, listenOptions =>
    {
        // Because not all routers, proxies, and etc support HTTP/3,
        // HTTP/3 should be configured together with HTTP/1.1 and HTTP/2
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;

        // HTTP/3 requires HTTPS
        listenOptions.UseHttps();
    });
});

await using var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// Register the Swagger generator, defining 1 or more Swagger documents
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
});

// Matches request to an endpoint
app.UseRouting();

// Execute the matched endpoint
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    // Implements greet.v1.Greeter
    endpoints.MapGrpcService<GreeterService>();
});

await app.RunAsync();
