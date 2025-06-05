using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GranHouseSA.Models
{
    public class Usuarios
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [DisplayName("Usuario")]
        public string NickName { get; set; }

        [Required]
        [DisplayName("Correo")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public char Rol { get; set; }
    }
}
