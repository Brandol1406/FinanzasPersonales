using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanzasPersonales.Models
{
    public class CuentaPres
    {
        public int id_cuent { get; set; }
        [Required(ErrorMessage = "El campo 'Presupuesto' es requerido")]
        public int id_pres { get; set; }
        [Required(ErrorMessage = "El campo 'Categoría' es requerido")]
        public int id_cat { get; set; }
        public string Categoria { get; set; }
        [Required(ErrorMessage = "El campo 'Límite' es requerido")]
        public decimal Limite { get; set; }
    }
    public class CuentaPresCreate
    {
        [Required(ErrorMessage = "El campo 'Presupuesto' es requerido")]
        public int id_pres { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "El campo 'Categoría' es requerido")]
        public int id_cat { get; set; }
        public string Categoria { get; set; }
        [Required(ErrorMessage = "El campo 'Límite' es requerido")]
        public decimal Limite { get; set; }
    }
    public class CuentaPresResumen
    {
        public int id_cuent { get; set; }
        public int id_pres { get; set; }
        public int id_cat { get; set; }
        public string Categoria { get; set; }
        public decimal Limite { get; set; }
        public decimal Gastado { get; set; }
        public decimal Porcentaje
        {
            get
            {
                return (Gastado / Limite) * 100;
            }
        }
        public string PorcentajeString
        {
            get
            {
                return ((Gastado / Limite) * 100).ToString("N2", Culture.Info);
            }
        }
    }
}