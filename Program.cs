using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connection = new SqliteConnection("DataSource=votos.db");
connection.Open();
builder.Services.AddDbContext<VotoDb>(opt => opt.UseSqlite(connection));

var app = builder.Build();

app.MapGet("/candidatos", (VotoDb _db) => TypedResults.Ok(_db.Candidatos.ToList()));
app.MapGet("/candidatos/{id}", async ([FromRoute] int id) => {
    var connection = new SqliteConnection("DataSource=votos.db");
    connection.Open();
    var options = new DbContextOptionsBuilder<VotoDb>()
    .UseSqlite(connection)
    .Options;
    var factory = new PooledDbContextFactory<VotoDb>(options);
    Candidato candidato = new(){
        Apellido = string.Empty,
        Nombre = string.Empty,
        Partido = string.Empty
    };
    using (var context = factory.CreateDbContext())
    {
        candidato = await context.Candidatos.FindAsync(id);
    }
    return TypedResults.Ok(candidato);
});
app.MapPost("/candidatos", (VotoDb _db, [FromBody] Candidato _nuevoCandidato) => {
    _db.Candidatos.Add(_nuevoCandidato);
    _db.SaveChanges();
    return TypedResults.Ok(_nuevoCandidato.Id);
});
app.MapPut("/candidatos/{id}", (VotoDb _db, [FromRoute] int id, [FromBody] Candidato _candidatoActualizado) =>  {
    var _candidato = _db.Candidatos.Find(id);
    _candidato.Nombre = _candidatoActualizado.Nombre;
    _candidato.Apellido = _candidatoActualizado.Apellido;
    _candidato.Partido = _candidatoActualizado.Partido;
    _db.SaveChanges();
    return TypedResults.Ok();
});
app.MapDelete("/candidatos/{id}", (VotoDb _db, [FromRoute] int id) => {
    var _candidato = _db.Candidatos.Find(id);
    _db.Candidatos.Remove(_candidato);
    _db.SaveChanges();
    return TypedResults.Ok();
});

app.Run();

public class Candidato
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public required string Partido { get; set; }
}

public class VotoDb : DbContext
{
    public VotoDb(DbContextOptions<VotoDb> options) : base(options) { 
        Database.EnsureCreatedAsync();
    }

    public DbSet<Candidato> Candidatos => Set<Candidato>();
}