using minimal_api.Dominio.Enums;
using minimal_api.Dominio.ModelView;

namespace minimal_api.Dominio.AdministradorLogado;

public record AdministradorLogado
{
	public string Email { get; set; } = default!;
	public string Perfil { get; set; } = default!;
	public string Token { get; set; } = default!;

}