using aspnet_qa.API.Mapping;
using aspnet_qa.API.Models;
using aspnet_qa.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Temel Servisler ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- 2. Swagger Yapılandırması (JWT Destekli) ---
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "QA API", Version = "v1" });

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

// --- 3. Veritabanı ve Repository Enjeksiyonu ---
// StackOverflow projesine uygun repository'leri ekliyoruz
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlCon"));
});

builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<QuestionRepository>();
builder.Services.AddScoped<AnswerRepository>();
builder.Services.AddScoped<TagRepository>();
builder.Services.AddScoped<VoteRepository>();

// --- 4. AutoMapper ---
builder.Services.AddAutoMapper(cfg => { }, typeof(MapProfile));

// --- 5. CORS (MVC projesinden AJAX istekleri için şart) ---
builder.Services.AddCors(p => p.AddPolicy("corspolicy", opt =>
{
    opt.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

// --- 6. Identity (Üyelik Sistemi) Ayarları ---
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 3;
})
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<AppDbContext>();

// --- 7. JWT Authentication (Kimlik Doğrulama) ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = false, // yayına aldığınızda bunu true yapmayı unutmayın!
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// --- HTTP İşlem Hattı (Pipeline) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("corspolicy");

app.UseAuthentication(); // Önce kimlik kontrolü
app.UseAuthorization();  // Sonra yetki kontrolü

app.MapControllers();

app.Run();