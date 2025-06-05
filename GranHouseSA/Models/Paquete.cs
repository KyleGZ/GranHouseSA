using System.ComponentModel.DataAnnotations;

namespace GranHouseSA.Models
{
    public class Paquete
    {

        [Key]
        public int ID { get; set; }
        public string NombrePaquete { get; set; }
        public double Costo { get; set; }

        public double Prima { get; set; }

        public int Mensualidad { get; set; }


    }
}
