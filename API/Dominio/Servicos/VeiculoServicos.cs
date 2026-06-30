using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.DB;
using minimal_api.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace minimal_api.Dominio.Servicos;

public class VeiculoServicos : IVeiculoServicos
{
    private readonly DBContexto _contexto;

    public VeiculoServicos(DBContexto contexto)
    {
        _contexto = contexto;
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public Veiculo? BuscaPorId(Veiculo veiculo)
    {
        return _contexto.Veiculos.Where(v => v.Id == veiculo.Id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _contexto.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome, $"%{nome}%"));
        }

        int itensPorPagina = 10;
        int paginaAtual = pagina ?? 1;

        query = query
            .OrderBy(v => v.Id)  // ← corrige o warning de Skip/Take sem OrderBy
            .Skip((paginaAtual - 1) * itensPorPagina)
            .Take(itensPorPagina);

        return query.ToList();
    }
}
