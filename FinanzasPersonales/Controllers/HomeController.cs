using FinanzasPersonales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanzasPersonales.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetResumenCuentas(int id_pres)
        {
            List<CuentaPresResumen> data = new List<CuentaPresResumen>();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                Data.t_presupuesto presupuesto = DB.t_presupuesto.Where(p => p.id_pres == id_pres).FirstOrDefault();
                if (presupuesto == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult { Success = false, Data = "Not Found!"});
                }
                List <CuentaPres> cuentas = (from c in DB.t_cuenta_pres
                                            join ca in DB.t_Categoria_gasto on c.id_cat equals ca.id_cat
                                            where c.id_pres == id_pres
                                            select new CuentaPres() { 
                                                id_cuent = c.id_cuent,
                                                id_pres = c.id_pres,
                                                id_cat = c.id_cat,
                                                Categoria = ca.Nombre,
                                                Limite = c.Limite
                                            }).ToList();
                List<Data.t_Gastos> gastos = DB.t_Gastos.Where(g => g.fecha >= presupuesto.Desde && g.fecha <= presupuesto.Hasta).ToList();
                foreach (var cuenta in cuentas)
                {
                    decimal gastado = gastos.Where(g => g.id_cat == cuenta.id_cat)?.Sum(g => g.valor) ?? 0 ;
                    data.Add(new CuentaPresResumen() { 
                        id_cuent = cuenta.id_cuent,
                        id_pres = cuenta.id_pres,
                        id_cat = cuenta.id_cat,
                        Categoria = cuenta.Categoria,
                        Limite = cuenta.Limite,
                        Gastado = gastado
                    });
                }
            }
            return Json(new ResponseResult() { Success = true, Data = new { List = data, TotalLimites = data.Sum(d => d.Limite), TotalGastado = data.Sum(d => d.Gastado) } });
        }
    }
}