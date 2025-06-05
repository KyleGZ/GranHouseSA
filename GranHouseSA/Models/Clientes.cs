using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GranHouseSA.Models
{
    public class Clientes
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Cedula { get; set; }

        [Required]
        [DisplayName("Tipo de cédula")]
        public string TipoCedula { get; set; }

        [Required]
        [DisplayName("Nombre completo")]
        public string Nombre { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        [DisplayName("Correo")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Por favor, introduce una dirección de correo electrónico válida.")]
        public string Email { get; set; }
    }
}
