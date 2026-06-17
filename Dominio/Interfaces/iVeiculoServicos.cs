using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.DTOs;

namespace minimal_api.Dominio.Interfaces;

public interface iVeiculoServicos
{
    List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null);
    Veiculo? BuscaPorId(Veiculo veiculo);
    void Incluir(Veiculo veiculo);
    void Atualizar(Veiculo veiculo);
    void Apagar(Veiculo veiculo);
}