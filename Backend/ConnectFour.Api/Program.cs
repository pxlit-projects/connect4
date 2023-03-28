using System.Reflection;
using System.Text;
using ConnectFour.Api.Filters;
using ConnectFour.Api.Services;
using ConnectFour.Api.Services.Contracts;
using ConnectFour.AppLogic;
using ConnectFour.AppLogic.Contracts;
using ConnectFour.Domain;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;
using ConnectFour.Domain.GridDomain.Contracts;
using ConnectFour.Domain.PlayerDomain;
using ConnectFour.Domain.PlayerDomain.Contracts;
using ConnectFour.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ConnectFour.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddSingleton(provider =>
            new ConnectFourExceptionFilterAttribute(provider.GetRequiredService<ILogger<Program>>()));

        builder.Services.AddControllers(options =>
        {
            var onlyAuthenticatedUsersPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build();
            options.Filters.Add(new AuthorizeFilter(onlyAuthenticatedUsersPolicy));
            options.Filters.AddService<ConnectFourExceptionFilterAttribute>();

            var jsonOutputFormatter = options.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().First();
            jsonOutputFormatter.SerializerOptions.Converters.Add(new TwoDimensionalArrayJsonConverter());

        });

        builder.Services.AddCors();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ConnectFour API",
                Description = "REST API for online Connect Four"
            });

            // Use XML documentation
            string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; //api project
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            xmlFilename = $"{typeof(IGrid).Assembly.GetName().Name}.xml"; //domain layer
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            xmlFilename = $"{typeof(IGameService).Assembly.GetName().Name}.xml"; //logic layer
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            // Enable bearer token authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Copy 'Bearer ' + valid token into field. You can retrieve a bearer token via '/api/authentication/token'"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        IConfiguration configuration = builder.Configuration;
        var tokenSettings = new TokenSettings();
        configuration.Bind("Token", tokenSettings);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = tokenSettings.Issuer,
                    ValidAudience = tokenSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<ConnectFourDbContext>(options =>
        {
            string connectionString = configuration.GetConnectionString("ConnectFourDbConnection");
            options.UseSqlServer(connectionString).EnableSensitiveDataLogging();
        });

        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 8;
                options.Lockout.AllowedForNewUsers = true;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 5;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<ConnectFourDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddSingleton<ITokenFactory>(new JwtTokenFactory(tokenSettings));
        builder.Services.AddScoped<IWaitingPool, WaitingPool>();
        builder.Services.AddSingleton<IGameCandidateFactory, GameCandidateFactory>();
        builder.Services.AddSingleton<IGameCandidateRepository, InMemoryGameCandidateRepository>();
        builder.Services.AddSingleton<IGameCandidateMatcher, BasicGameCandidateMatcher>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IGameFactory>(services => new GameFactory(services.GetRequiredService<IGamePlayStrategy>()) as IGameFactory);
        builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
        int miniMaxSearchDepth = builder.Configuration.GetValue<int>("MiniMaxSearchDepth");
        builder.Services.AddScoped<IGamePlayStrategy, MiniMaxGamePlayStrategy>(services =>
            new MiniMaxGamePlayStrategy(services.GetRequiredService<IGridEvaluator>(), miniMaxSearchDepth));
        builder.Services.AddScoped<IGridEvaluator>(_ => new GridEvaluator() as IGridEvaluator);

        var app = builder.Build();


        //Create database (if it does not exist yet)
        var scope = app.Services.CreateScope();
        ConnectFourDbContext context = scope.ServiceProvider.GetRequiredService<ConnectFourDbContext>();
        context.Database.EnsureCreated();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader());
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}