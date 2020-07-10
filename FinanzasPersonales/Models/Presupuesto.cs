using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinanzasPersonales.Models
{
    public class Presupuesto
    {
        public int id_pres { get; set; }
        [Required(ErrorMessage = "El campo 'Desde' es requerido")]
        public DateTime Desde { get; set; }
        [Required(ErrorMessage = "El campo 'Hasta' es requerido")]
        public DateTime Hasta { get; set; }
        public string Nombre { get; set; }
        public string Representacion 
        { 
            get 
            { 
                return $"Del {Desde.ToString("dd/MM/yyyy", Culture.Info)} al { Hasta.ToString("dd/MM/yyyy", Culture.Info) }, { Nombre }"; 
            } 
        }
    }
    public class PresupuestoCreate
    {
        [Required(ErrorMessage = "El campo 'Desde' es requerido")]
        public DateTime Desde { get; set; }
        [Required(ErrorMessage = "El campo 'Hasta' es requerido")]
        public DateTime Hasta { get; set; }
        public string Nombre { get; set; }
        public string Representacion
        {
            get
            {
                return $"Del {Desde.ToString("dd/MM/yyyy", Culture.Info)} al { Hasta.ToString("dd/MM/yyyy", Culture.Info) }, { Nombre }";
            }
        }
    }
}