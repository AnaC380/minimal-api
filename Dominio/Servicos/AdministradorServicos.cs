using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.DB;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Servicos;

public class AdministradorServicos : IAdministradorServicos
{
    private readonly DBContexto _contexto;

    public AdministradorServicos(DBContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? BuscaPorId(int id)
    {
        return _contexto.Administradores.FirstOrDefault(v => v.Id == id);
    }

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return _contexto.Administradores
            .FirstOrDefault(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Password);
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();

        int itensPorPagina = 10;
        int paginaAtual = pagina ?? 1;

        query = query
        .OrderBy(a => a.Id)
        .Skip((paginaAtual - 1) * itensPorPagina)
        .Take(itensPorPagina);

        return query.ToList();
    }
}