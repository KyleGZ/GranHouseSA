using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GranHouseSA.Controllers;

namespace GranHouseSA.Models
{
    public class Compras
    {
        PaquetesController _paquetes;

        public Compras()
        {
            _paquetes = new PaquetesController();
        }

        [Key]
        [DisplayName("ID Factura")]
        public int IDFactura { get; set; }

        [Required(ErrorMessage = "Ingrese la cédula")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre")]
        [DisplayName("Nombre del cliente")]
        public string nombreCliente { get; set; }

        [Required]
        [DisplayName("ID del paquete")]
        public int IDPaquete { get; set; }

        [Required]
        [DisplayName("Nombre paquete")]
        public string nombrePaquete { get; set; }

        [Required(ErrorMessage = "Ingrese la cantidad de noches")]
        [DisplayName("Noches")]
        [Range(0, double.MaxValue, ErrorMessage = "La cantidad debe ser un valor no negativo.")]
        public int CantidadNoches { get; set; }

        public string Transaccion { get; set; }

        [Required(ErrorMessage = "Ingrese una fecha")]
        [DisplayName("Fecha ingreso")]
        public DateTime FechaIngreso { get; set; }

        [DisplayName("Subtotal")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalPagar { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Descuento
        {
            get
            {
                var descuento=0.0;

                if (Transaccion != null)
                {
                    //Si la transacción es efectivo, se aplica el descuento
                    if (Transaccion.Equals("Efectivo"))
                    {
                        if (CantidadNoches >= 3 && CantidadNoches <= 6)
                        {
                            descuento = TotalPagar * 0.10;
                        }

                        if (CantidadNoches >= 7 && CantidadNoches <= 9)
                        {
                            descuento = TotalPagar * 0.15;
                        }

                        if (CantidadNoches >= 10 && CantidadNoches <= 12)
                        {
                            descuento = TotalPagar * 0.20;
                        }

                        if (CantidadNoches >= 13)
                        {
                            descuento = TotalPagar * 0.25;
                        }
                    }
                }

                return descuento;
            }
        }

        [DisplayName("Monto final en dolares")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double MontoFinal
        {
            get
            {
                //Se le resta el descuento y a la vez se le suma el IVA
                return TotalPagar - Descuento + (TotalPagar - Descuento) * 0.13;
            }
        }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Prima { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Mensualidad { get; set; }
    }
}
