using System;
using System.Collections.Generic;
using minimal_api_desafio;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api_desafio.Domain.DTOs;
using minimal_api_desafio.Domain.Entities;
using minimal_api_desafio.Domain.Enuns;
using minimal_api_desafio.Domain.Interfaces;
using minimal_api_desafio.Domain.ModelViews;
using minimal_api_desafio.Domain.Services;
using minimal_api_desafio.Infra.Db;

namespace minimal_api_desafio
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration.GetSection("Jwt").GetChildren().ToString() ?? "123456";
        }

        private string key;
        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
            services.AddAuthorization();

            services.AddScoped<IAdmService, AdmService>();
            services.AddScoped<IVeiculoService, VeiculoService>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token jwt ",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                },
                new string[] {}
                }
              });
            });

            services.AddDbContext<ProjContext>((options) =>
            {
                options
            .UseSqlServer(connectionString);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                string GerarTokenJwt(Adm adm)
                {
                    if (string.IsNullOrEmpty(key)) return string.Empty;
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>(){
                        new Claim("Email", adm.Email),
                        new Claim("Perfil", adm.Perfil),
                        new Claim(ClaimTypes.Role, adm.Perfil),
                    };

                    var token = new JwtSecurityToken(
                      claims: claims,
                      expires: DateTime.Now.AddDays(1),
                      signingCredentials: credentials
                      );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                endpoints.MapPost("/login", ([FromBody] LoginDTO loginDTO, [FromServices] IAdmService admService) =>
                {
                    var adm = admService.Login(loginDTO);
                    if (adm is not null)
                    {
                        string token = GerarTokenJwt(adm);
                        return Results.Ok(new AdmLogado
                        {
                            Email = adm.Email,
                            Id = adm.Id,
                            Perfil = adm.Perfil,
                            Token = token
                        });
                    }
                    else return Results.Unauthorized();
                }).AllowAnonymous().WithTags("Autenticação");

                ErrosDeValidacao validaAdmDTO(AdmistradorDTO admistradorDTO)
                {
                    var validacao = new ErrosDeValidacao();

                    if (string.IsNullOrEmpty(admistradorDTO.Email))
                        validacao.Mensagens.Add("Erro email vazio");
                    if (string.IsNullOrEmpty(admistradorDTO.Perfil.ToString())) validacao.Mensagens.Add("Erro perfil vazia");
                    if (string.IsNullOrEmpty(admistradorDTO.Senha)) validacao.Mensagens.Add("Erro senha vazia");
                    if (admistradorDTO.Senha.Length < 6) validacao.Mensagens.Add("Erro senha tem que ser maior que 6 digitos");

                    return validacao;
                }

                endpoints.MapPost("/admistradores/cadastrar", ([FromBody] AdmistradorDTO admistradorDTO, [FromServices] IAdmService admService) =>
                {
                    var validacao = validaAdmDTO(admistradorDTO);

                    if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);
                    var adm = new Adm
                    {
                        Email = admistradorDTO.Email,
                        Perfil = admistradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
                        Senha = admistradorDTO.Senha,
                    };
                    admService.Incluir(adm);
                    return Results.Created($"/admistradores/{adm.Id}", new AdministradorModelView
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Id = adm.Id,
                    });
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Admistradores");

                endpoints.MapGet("/admistradores", ([FromServices] IAdmService admService, [FromQuery] int pagina = 1) =>
                {
                    var resp = new List<AdministradorModelView>();
                    var adms = admService.Todos(pagina);
                    foreach (var adm in adms)
                    {
                        resp.Add(new AdministradorModelView
                        {
                            Id = adm.Id,
                            Email = adm.Email,
                            Perfil = adm.Perfil
                        });
                    }
                    return Results.Ok(resp);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admistradores");

                ErrosDeValidacao validaVeiculoDTO(VeiculoDTO veiculoDTO)
                {
                    var validacao = new ErrosDeValidacao();

                    if (string.IsNullOrEmpty(veiculoDTO.Nome))
                        validacao.Mensagens.Add("Erro nome vazio");
                    if (string.IsNullOrEmpty(veiculoDTO.Marca)) validacao.Mensagens.Add("Erro marca vazia");
                    if (veiculoDTO.Ano < 1945) validacao.Mensagens.Add("Erro ano deve ser maior ou igual a 1945");

                    return validacao;
                }

                endpoints.MapGet("/admistradores/{id}", ([FromServices] IAdmService admService, [FromRoute] int id) =>
                {
                    var adm = admService.BuscaPorId(id);
                    if (adm is null) return Results.NotFound();
                    return Results.Ok(new AdministradorModelView { Email = adm.Email, Perfil = adm.Perfil, Id = adm.Id });
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admistradores");

                endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, [FromServices] IVeiculoService veiculoService) =>
                {
                    var validacao = validaVeiculoDTO(veiculoDTO);

                    if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

                    var veiculo = new Veiculo
                    {
                        Nome = veiculoDTO.Nome,
                        Ano = veiculoDTO.Ano,
                        Marca = veiculoDTO.Marca,
                    };
                    veiculoService.Incluir(veiculo);
                    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");

                endpoints.MapGet("/veiculos", ([FromServices] IVeiculoService veiculoService, [FromQuery] int pagina = 1) =>
                {
                    var veiculos = veiculoService.Todos(pagina);
                    return Results.Ok(veiculos);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");

                endpoints.MapGet("/veiculos/{id}", ([FromServices] IVeiculoService veiculoService, [FromRoute] int id) =>
                {
                    var veiculo = veiculoService.BuscaPorId(id);
                    if (veiculo is null) return Results.NotFound();
                    return Results.Ok(veiculo);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");

                endpoints.MapPut("/veiculos/{id}", ([FromBody] VeiculoDTO veiculoDTO, [FromServices] IVeiculoService veiculoService, [FromRoute] int id) =>
                {
                    var validacao = validaVeiculoDTO(veiculoDTO);

                    var veiculo = veiculoService.BuscaPorId(id);
                    if (veiculo is null) return Results.NotFound();

                    if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

                    veiculo.Nome = veiculoDTO.Nome;
                    veiculo.Ano = veiculoDTO.Ano;
                    veiculo.Marca = veiculoDTO.Marca;

                    veiculoService.Atualizar(veiculo);

                    return Results.Ok(veiculo);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veiculos");

                endpoints.MapDelete("/veiculos/{id}", ([FromServices] IVeiculoService veiculoService, [FromRoute] int id) =>
                {
                    var veiculo = veiculoService.BuscaPorId(id);
                    if (veiculo is null) return Results.NotFound();

                    veiculoService.Apagar(veiculo);

                    return Results.NoContent();
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veiculos");
            });
        }
    }
}