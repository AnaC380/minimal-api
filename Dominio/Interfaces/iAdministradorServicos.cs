using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.DTOs;

namespace minimal_api.Dominio.Interfaces;

public interface iAdministradorServicos
{
    bool Login(LoginDTO loginDTO);
}