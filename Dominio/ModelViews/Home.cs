namespace minimal_api.Dominio.ModelView
{
    public struct Home
    {
        public string Documentacao { get => "/swagger"; }

        public string Mensagem { get => "Bem-vindo à API de Veículos - Minimal API"; }
    }
}