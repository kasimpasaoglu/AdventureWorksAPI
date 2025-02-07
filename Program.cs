using System.Text;
using AdventureWorksAPI.DataAccessLayer;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is null or empty!");
}



builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// DATABASE CONNECTION
builder.Services.AddDbContext<AdventureWorksContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();


#region Swagger
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
#endregion 


#region CORS
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
#endregion


#region JWT
var JwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtSettings["Issuer"],
        ValidAudience = JwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings["Key"]))
    };
});
#endregion

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
app.MapControllers();



app.Run();


