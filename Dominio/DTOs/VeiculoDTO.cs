namespace minimal_api.Dominio.DTOs;

public record VeiculoDTO
{

    public int Id { get; set; } = default!;

    public string Nome { get; set; } = default!;

    public string Marca { get; set; } = default!;

    public int Ano { get; set; } = default!;
}
