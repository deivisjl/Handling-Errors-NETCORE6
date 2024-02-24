using API.Extensions;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);

var secretKey = Encoding.ASCII.GetBytes("DwkdopIDAISOPDQWD59AS8D9AWD2ASD9sd59qwd");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };

    x.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();

            var httpContext = context.HttpContext;
            var statusCode = StatusCodes.Status401Unauthorized;

            var routeData = httpContext.GetRouteData();
            var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

            var factory = httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problemDetails = factory.CreateProblemDetails(httpContext, statusCode);
            var details = new ErrorDetails
            {
                Message = problemDetails.Title,
                StatusCode = problemDetails.Status
            };

            var result = new ObjectResult(details) { StatusCode = statusCode };
            await result.ExecuteResultAsync(actionContext);
        }
    };
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
