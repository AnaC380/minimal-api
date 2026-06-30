using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using minimal_api.Dominio.Enums;

namespace minimal_api.Dominio.Entidades;

public class Administrador
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Senha { get; set; } = default!;

    [Required]
    [StringLength(10)]
    public Perfil Perfil { get; set; } = default!;
}
