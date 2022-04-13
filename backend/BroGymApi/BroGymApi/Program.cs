using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BroGymApi.Data;
using BroGymApi.Interfaces;
using BroGymApi.Repositories;
using BroGymApi.Services;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------- Services -----------------------------------------------------

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BroGymApi", Version = "v1" });

    // https://stackoverflow.com/questions/43447688/setting-up-swagger-asp-net-core-using-the-authorization-headers-bearer
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Coloque **_apenas_** o token (JWT Bearer) abaixo!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// Inserir as informa��es do banco na vari�vel builder antes de build�-la;
//var secretSenhaBancoDados = builder.Configuration["SecretSenhaBancoDados"]; // secrets.json;
string con = builder.Configuration.GetConnectionString("BDBroGym");
//con = con.Replace("[secretSenhaBancoDados]", secretSenhaBancoDados); // Alterar pela senha do secrets.json;
builder.Services.AddDbContext<Context>(options => options.UseMySql(con, ServerVersion.AutoDetect(con)));

// Banco (injection) com o Pattern Design: https://www.c-sharpcorner.com/blogs/net-core-mvc-with-entity-framework-core-using-dependency-injection-and-repository
// NOVOS REPOSIT�RIOS DEVEM SEMPRE SER ADICIONADOS AQUI;
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Outros;
builder.Services.AddMvc();

// Cors;
builder.Services.AddCors();

// Autentica��o JWT para a API: https://balta.io/artigos/aspnet-5-autenticacao-autorizacao-bearer-jwt;
var key = Encoding.ASCII.GetBytes(Chave.chave);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Etc (padr�o);
// Adicionar comando "AddNewtonsoftJson" para ignorar "erros" de object cycle - https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported;
// E tamb�m formatar o resultado JSON retornado pelas APIs;
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
});

// Verificando a disponibilidade dos bancos de dados da aplica��o atrav�s de Health Checks: https://www.treinaweb.com.br/blog/verificando-a-integridade-da-aplicacao-asp-net-core-com-health-checks
builder.Services.AddHealthChecks();

// ----------------------------------------------------- APP -----------------------------------------------------

var app = builder.Build();

// Iniciar banco;
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<Context>();

        // Iniciar o banco de dados, indo para o DbInitializer.cs;
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        string erroBD = ex.Message.ToString();
    }
}

// Configure the HTTP request pipeline;
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BromGymApi v1");
    c.DocExpansion(DocExpansion.None); // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/279
    }
);
//}

// Cors;
app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());

// Ativa o HealthChecks;
app.UseHealthChecks("/status");

// Outros;
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Exibir erros;
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Habilitar static files para exibir as imagens da API: https://youtu.be/jSO5KJLd5Qk?t=86;
//IWebHostEnvironment env = app.Environment;
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Upload")),
//    RequestPath = "/Upload"
//});

