using FinanzasPersonales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanzasPersonales.Controllers
{
    public class GastosController : Controller
    {
        // Registro Gastos
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(GastoCreateModel model)
        {
            if (ModelState.IsValid)
            {
                using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
                {
                    var presInfo = (from p in DB.t_presupuesto
                                    join c in DB.t_cuenta_pres on p.id_pres equals c.id_pres
                                    where (p.Desde <= model.fecha && p.Hasta >= model.fecha) && c.id_cat == model.categoria
                                    select new
                                    {
                                        Desde = p.Desde,
                                        Hasta = p.Hasta,
                                        id_pres = p.id_pres,
                                        id_cuent = c.id_cuent,
                                        id_cat = c.id_cat,
                                        Limite = c.Limite
                                    }).FirstOrDefault();
                    if (presInfo is null)
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                            new ValidationResult() { Key = "id_gasto", Errors = new ModelErrorCollection() { "No existe una cuenta presupuestaria con esta categoría de gasto que abarque esta fecha" }}} }
                        );
                    }

                    var gastosCat = DB.t_Gastos.Where(g => (g.fecha >= presInfo.Desde && g.fecha <= presInfo.Hasta) && g.id_cat == model.categoria).ToList();
                    decimal gastado = (gastosCat.Count == 0 ? 0 : gastosCat.Sum(g => g.valor));
                    decimal disponible = presInfo.Limite - gastado;
                    if (model.valor > disponible)
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                            new ValidationResult() { Key = "id_gasto", Errors = new ModelErrorCollection() { "El valor de este gasto excede el disponible de la cuenta presupuestaria" }}}
                        }
                        );
                    }
                    DB.t_Gastos.Add(new Data.t_Gastos()
                    {
                        id_cat = model.categoria,
                        fecha = model.fecha,
                        justificacion = model.justificacion,
                        valor = model.valor
                    });
                    DB.SaveChanges();
                }
                return Json(new ResponseResult() { Success = true, Data = Validations.GetErrors(ModelState) });
            }
            else
            {
                //Reject
                return Json(new ResponseResult() { Success = false, Data = Validations.GetErrors(ModelState) });
            }
        }
        [HttpPost]
        public JsonResult Edit(GastoModel model)
        {
            if (ModelState.IsValid)
            {
                //Save
                using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
                {
                    var presInfo = (from p in DB.t_presupuesto
                                    join c in DB.t_cuenta_pres on p.id_pres equals c.id_pres
                                    where (p.Desde <= model.fecha && p.Hasta >= model.fecha) && c.id_cat == model.categoria
                                    select new
                                    {
                                        Desde = p.Desde,
                                        Hasta = p.Hasta,
                                        id_pres = p.id_pres,
                                        id_cuent = c.id_cuent,
                                        id_cat = c.id_cat,
                                        Limite = c.Limite
                                    }).FirstOrDefault();
                    if (presInfo is null)
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                            new ValidationResult() { Key = "id_gasto", Errors = new ModelErrorCollection() { "No existe una cuenta presupuestaria con esta categoría de gasto que abarque esta fecha" }}}
                        }
                        );
                    }

                    var gastosCat = DB.t_Gastos.Where(g => (g.fecha >= presInfo.Desde && g.fecha <= presInfo.Hasta) && g.id_cat == model.categoria && g.id_gasto != model.id_gasto).ToList();
                    decimal gastado = (gastosCat.Count == 0 ? 0 : gastosCat.Sum(g => g.valor));
                    decimal disponible = presInfo.Limite - gastado;

                    if (model.valor > disponible)
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                            new ValidationResult() { Key = "id_gasto", Errors = new ModelErrorCollection() { "El valor de este gasto excede el disponible de la cuenta presupuestaria" }}}
                        }
                        );
                    }

                    Data.t_Gastos gasto = DB.t_Gastos.Where(g => g.id_gasto == model.id_gasto).FirstOrDefault();

                    gasto.id_cat = model.categoria;
                    gasto.fecha = model.fecha;
                    gasto.justificacion = model.justificacion;
                    gasto.valor = model.valor;

                    DB.SaveChanges();
                }
                return Json(new ResponseResult() { Success = true, Data = Validations.GetErrors(ModelState) });
            }
            else
            {
                //Reject
                return Json(new ResponseResult() { Success = false, Data = Validations.GetErrors(ModelState) });
            }
        }
        [HttpPost]
        public JsonResult Get(int id)
        {
            GastoModel gasto = new GastoModel();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                var fromDbGasto = DB.t_Gastos.Where(g => g.id_gasto == id).FirstOrDefault();
                if (fromDbGasto == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult() { Success = false , Data = "Not found"});
                }
                else
                {
                    gasto = new GastoModel() {
                        id_gasto = fromDbGasto.id_gasto,
                        categoria = fromDbGasto.id_cat,
                        justificacion = fromDbGasto.justificacion,
                        fecha = fromDbGasto.fecha,
                        valor = fromDbGasto.valor
                    };
                }
            }
            return Json(gasto);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            GastoCreateModel gasto = new GastoCreateModel();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                var fromDbGasto = DB.t_Gastos.Where(g => g.id_gasto == id).FirstOrDefault();
                if (fromDbGasto == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult() { Success = false , Data = "Not found"});
                }
                else
                {
                    DB.t_Gastos.Remove(fromDbGasto);
                    DB.SaveChanges();
                }
            }
            return Json(gasto);
        }
        [HttpPost]
        public JsonResult GetCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                categorias.AddRange(DB.t_Categoria_gasto.Select(c => new Categoria()
                {
                    id_cat = c.id_cat,
                    Nombre = c.Nombre
                }));
            }
            return Json(categorias);
        }
        [HttpGet]
        public string GetGastos()
        {
            List<GastoModel> gastos = new List<GastoModel>();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                gastos.AddRange(from g in DB.t_Gastos
                                join c in DB.t_Categoria_gasto on g.id_cat equals c.id_cat
                                select new GastoModel() {
                                    id_gasto = g.id_gasto,
                                    categoriaNombre = c.Nombre,
                                    categoria = g.id_cat,
                                    fecha = g.fecha,
                                    valor = g.valor,
                                    justificacion = g.justificacion
                                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(gastos);
        }
        // Reporte Gastos
        public ActionResult Reporte()
        {
            return View();
        }
        [HttpPost]
        public string GetGastos(int categoria, DateTime desde, DateTime hasta)
        {
            List<GastoModel> gastos = new List<GastoModel>();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                gastos.AddRange(from g in DB.t_Gastos
                                join c in DB.t_Categoria_gasto on g.id_cat equals c.id_cat
                                where
                                    (g.fecha >= desde && g.fecha <= hasta)
                                    &&
                                    (g.id_cat == categoria || categoria == 0)
                                select new GastoModel()
                                {
                                    id_gasto = g.id_gasto,
                                    categoriaNombre = c.Nombre,
                                    categoria = g.id_cat,
                                    fecha = g.fecha,
                                    valor = g.valor,
                                    justificacion = g.justificacion
                                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { list = gastos.OrderByDescending(g => g.fecha).ToList(), total = gastos.Sum(g => g.valor), numero = gastos.Count });
        }
    }
}