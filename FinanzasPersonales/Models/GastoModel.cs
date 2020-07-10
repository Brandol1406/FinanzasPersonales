using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FinanzasPersonales.Models
{
    public class GastoModel
    {
        public int id_gasto { get; set; }
        public string categoriaNombre { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una Categoría")]
        public int categoria { get; set; }
        [Required(ErrorMessage = "El campo 'Justificación' es requerido")]
        public string justificacion { get; set; }
        [Required(ErrorMessage = "El campo 'Fecha' es requerido")]
        public DateTime fecha { get; set; }
        [Required(ErrorMessage = "El campo 'Valor' es requerido")]
        public decimal valor { get; set; }
    }
    public class GastoCreateModel
    {
        public string categoriaNombre { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una Categoría")]
        public int categoria { get; set; }
        [Required(ErrorMessage = "El campo 'Justificación' es requerido")]
        public string justificacion { get; set; }
        [Required(ErrorMessage = "El campo 'Fecha' es requerido")]
        public DateTime fecha { get; set; }
        [Required(ErrorMessage = "El campo 'Valor' es requerido")]
        public decimal valor { get; set; }
    }
}