using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
List<Candidato> _candidatos = new();

app.MapGet("/candidatos", () => TypedResults.Ok(_candidatos));
app.MapGet("/candidatos/{id}", ([FromRoute] int id) => TypedResults.Ok(_candidatos.FirstOrDefault(c => c.Id == id)));
app.MapPost("/candidatos", ([FromBody] Candidato _nuevoCandidato) => {
    _nuevoCandidato.Id = _candidatos.Count + 1;
    _candidatos.Add(_nuevoCandidato);
    return TypedResults.Ok(_nuevoCandidato.Id);
});
app.MapPut("/candidatos/{id}", ([FromRoute] int id, [FromBody] Candidato _candidatoActualizado) =>  {
    foreach (var _candidato in _candidatos)
    {
        if (_candidato.Id == id) 
        {
            _candidato.Nombre = _candidatoActualizado.Nombre;
            _candidato.Apellido = _candidatoActualizado.Apellido;
            _candidato.Partido = _candidatoActualizado.Partido;
        }
    }
    return TypedResults.Ok();
});
app.MapDelete("/candidatos/{id}", ([FromRoute] int id) => {
    _candidatos = _candidatos.Where(c => c.Id != id).ToList();
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