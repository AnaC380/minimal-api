using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.DB;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Servicos;

public class AdministradorServicos : iAdministradorServicos
{
    private readonly DBContexto _contexto;

    public AdministradorServicos(DBContexto contexto)
    {
        _contexto = contexto;
    }

    public bool Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores
            .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Password)
            .FirstOrDefault();

        return adm != null;
    }
}