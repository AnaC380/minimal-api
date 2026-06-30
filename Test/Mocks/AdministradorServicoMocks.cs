using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enums;
using minimal_api.Dominio.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace Test.Mocks   
{
    internal class AdministradorServicoMocks : IAdministradorServicos
    {
        private static List<Administrador> administradores = new List<Administrador>()
        {
            new Administrador
            {
                Id = 1,
                Email = "adm@teste.com",
                Senha = BC.HashPassword("123456"),
                Perfil = Perfil.Adm
            },
            new Administrador
            {
                Id = 2,
                Email = "editor@teste.com",
                Senha = BC.HashPassword("123456"),
                Perfil = Perfil.Editor
            },

        };
        public AdministradorServicoMocks()
        {
        }

        public Administrador? BuscaPorId(int id)
        {
            return administradores.Find(a => a.Id == id);
        }

        public Administrador Incluir(Administrador administrador)
        {
            administrador.Id = administradores.Count + 1;
            administradores.Add(administrador);
            return administrador;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            return administradores.Find(a =>
                a.Email == loginDTO.Email &&
                BC.Verify(loginDTO.Password, a.Senha));
        }

        public static List<Administrador> Todos() 
        {
            return administradores;
        }

        public List<Administrador> Todos(int? pagina)
        {
            throw new NotImplementedException();
        }
    }
}
