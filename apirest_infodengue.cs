using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoDengueAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=your_server;Database=InfoDengueDB;User Id=your_user;Password=your_password;"));
            services.AddScoped<ISolicitanteService, SolicitanteService>();
            services.AddScoped<IRelatorioService, RelatorioService>();
            services.AddHttpClient();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Solicitante> Solicitantes { get; set; }
        public DbSet<Relatorio> Relatorios { get; set; }
    }

    public class Solicitante
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public ICollection<Relatorio> Relatorios { get; set; }
    }

    public class Relatorio
    {
        public int Id { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string Arbovirose { get; set; }
        public int SemanaInicio { get; set; }
        public int SemanaTermino { get; set; }
        public int CodigoIBGE { get; set; }
        public string Municipio { get; set; }
        public int SolicitanteId { get; set; }
        public Solicitante Solicitante { get; set; }
    }

    public interface ISolicitanteService
    {
        Task<Solicitante> AddOrUpdateSolicitanteAsync(string nome, string cpf);
        Task<List<Solicitante>> GetAllSolicitantesAsync();
    }

    public class SolicitanteService : ISolicitanteService
    {
        private readonly AppDbContext _context;

        public SolicitanteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Solicitante> AddOrUpdateSolicitanteAsync(string nome, string cpf)
        {
            var solicitante = await _context.Solicitantes.FirstOrDefaultAsync(s => s.CPF == cpf);
            if (solicitante == null)
            {
                solicitante = new Solicitante { Nome = nome, CPF = cpf };
                _context.Solicitantes.Add(solicitante);
                await _context.SaveChangesAsync();
            }
            return solicitante;
        }

        public async Task<List<Solicitante>> GetAllSolicitantesAsync()
        {
            return await _context.Solicitantes.ToListAsync();
        }
    }

    public interface IRelatorioService
    {
        Task<Relatorio> CreateRelatorioAsync(Relatorio relatorio);
        Task<List<Relatorio>> GetAllRelatoriosAsync();
        Task<List<Relatorio>> GetRelatoriosByMunicipioAsync(string municipio);
    }

    public class RelatorioService : IRelatorioService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public RelatorioService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Relatorio> CreateRelatorioAsync(Relatorio relatorio)
        {
            _context.Relatorios.Add(relatorio);
            await _context.SaveChangesAsync();
            return relatorio;
        }

        public async Task<List<Relatorio>> GetAllRelatoriosAsync()
        {
            return await _context.Relatorios.Include(r => r.Solicitante).ToListAsync();
        }

        public async Task<List<Relatorio>> GetRelatoriosByMunicipioAsync(string municipio)
        {
            return await _context.Relatorios.Where(r => r.Municipio == municipio).ToListAsync();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SolicitantesController : ControllerBase
    {
        private readonly ISolicitanteService _solicitanteService;

        public SolicitantesController(ISolicitanteService solicitanteService)
        {
            _solicitanteService = solicitanteService;
        }

        [HttpPost]
        public async Task<IActionResult> AddSolicitante([FromBody] Solicitante solicitante)
        {
            var result = await _solicitanteService.AddOrUpdateSolicitanteAsync(solicitante.Nome, solicitante.CPF);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSolicitantes()
        {
            var result = await _solicitanteService.GetAllSolicitantesAsync();
            return Ok(result);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpPost]
        public async Task<IActionResult> AddRelatorio([FromBody] Relatorio relatorio)
        {
            var result = await _relatorioService.CreateRelatorioAsync(relatorio);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRelatorios()
        {
            var result = await _relatorioService.GetAllRelatoriosAsync();
            return Ok(result);
        }

        [HttpGet("municipio/{municipio}")]
        public async Task<IActionResult> GetRelatoriosByMunicipio(string municipio)
        {
            var result = await _relatorioService.GetRelatoriosByMunicipioAsync(municipio);
            return Ok(result);
        }
    }
}
