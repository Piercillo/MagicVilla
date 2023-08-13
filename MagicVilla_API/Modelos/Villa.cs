using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Modelos
{
    //este es el modelo
    public class Villa
    {//presionando prop mas tab se crea la linea de abajao acuerdate
        //clave primaria
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//controlara que se asigne autolnaticamente el id
        public int Id { get; set; }

        public string Nombre { get; set; }



        public string Detalle { get; set; }

        [Required]
        public double Tarifa { get; set; }

        public int Ocupantes { get; set; }

        public int MetrosCuadrados { get; set; }

        public string ImagenUrl { get; set; }

        public string Amenidad { get; set; }

        public DateTime FechaCreacion { get; set; } //fecha de creacion

        public DateTime FechaActualizacion { get; set; }

    }



}
