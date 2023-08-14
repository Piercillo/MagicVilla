using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Modelos.Dto
{
    public class VillaCreateDto
    {       //ESTO ES PARA Evitar directamente con nuestro modelo de la base de datos
        //clase do data trasnfer object

        [Required] //si no te agarra Ctr + .
        [MaxLength(30)]
        public string Nombre { get; set; } //aqui no uso la fecha de creacion para exponer

        public string Detalle { get; set; }

        [Required]
        public double Tarifa { get; set; }

        public int Ocupantes { get; set; }

        public int MetrosCuadrados { get; set; }
        
        public string ImagenUrl { get; set; }

        public string Amenidad { get; set; }

    }


}
