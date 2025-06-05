using GranHouseSA.Controllers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GranHouseSA.Models
{
    public class Reservacion
    {
        PaquetesController _paquetes;

        public Reservacion()
        {
            _paquetes = new PaquetesController();
        }

        [Key]
        public int IDReservacion { get; set; }

        [Required(ErrorMessage = "Ingrese la cédula")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre")]
        [DisplayName("Nombre del cliente")]
        public string NombreCliente { get; set; }

        [Required]
        [DisplayName("ID del paquete")]
        public int IDPaquete { get; set; }

        [Required]
        [DisplayName("Nombre del paquete")]
        public string nombrePaquete
        {
            get
            {
                //Me trae el nombre del paquete en tiempo real desde la api, por este cambia.
                var temp = _paquetes.GetPaquete(IDPaquete);
                return temp.NombrePaquete;
            }
        }
    }
}
