using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanzasPersonales.Models
{
    public class Categoria
    {
        public int id_cat { get; set; }
        [Required(ErrorMessage = "El campo 'Nombre Categoría' es requerido")]
        public string Nombre { get; set; }
    }
    public class CategoriaCreate
    {
        [Required(ErrorMessage = "El campo 'Nombre Categoría' es requerido")]
        public string Nombre { get; set; }
    }
}