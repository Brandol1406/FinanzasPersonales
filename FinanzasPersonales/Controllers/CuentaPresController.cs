using FinanzasPersonales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanzasPersonales.Controllers
{
    public class CuentaPresController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(CuentaPresCreate model)
        {
            if (ModelState.IsValid)
            {
                //Save
                using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
                {
                    //Validando que solo exista una cuenta presupuestaria para esta categoria
                    if (DB.t_cuenta_pres.Any(a => a.id_cat == model.id_cat && a.id_pres == model.id_pres))
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                                new ValidationResult() {
                                    Key = "id_cat",
                                    Errors = new ModelErrorCollection() { "Ya existe esta categoria de gasto en este presupuesto" }
                                }
                            }
                        });
                    }
                    DB.t_cuenta_pres.Add(new Data.t_cuenta_pres()
                    {
                        id_pres = model.id_pres,
                        id_cat = model.id_cat,
                        Limite = model.Limite
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
        public JsonResult Edit(CuentaPres model)
        {
            if (ModelState.IsValid)
            {
                //Save
                using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
                {
                    //Validando que solo exista una cuenta presupuestaria para esta categoria
                    if (DB.t_cuenta_pres.Any(a => (a.id_cat == model.id_cat && a.id_pres == model.id_pres) && a.id_cuent != model.id_cuent))
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                                new ValidationResult() {
                                    Key = "id_cat",
                                    Errors = new ModelErrorCollection() { "Ya existe esta categoria de gasto en este presupuesto" }
                                }
                            }
                        });
                    }
                    Data.t_cuenta_pres data = DB.t_cuenta_pres.Where(c => c.id_cuent == model.id_cuent).FirstOrDefault();

                    data.id_pres = model.id_pres;
                    data.id_cat = model.id_cat;
                    data.Limite = model.Limite;

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
            CuentaPres data = new CuentaPres();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                var fromDbCat = DB.t_cuenta_pres.Where(c => c.id_cuent == id).FirstOrDefault();
                if (fromDbCat == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult() { Success = false, Data = "Not found" });
                }
                else
                {
                    data = new CuentaPres()
                    {
                        id_cuent = fromDbCat.id_cuent,
                        id_pres = fromDbCat.id_pres,
                        id_cat = fromDbCat.id_cat,
                        Limite = fromDbCat.Limite,
                        Categoria = DB.t_Categoria_gasto.Where(c => c.id_cat == fromDbCat.id_cat).FirstOrDefault().Nombre
                    };
                }
            }
            return Json(data);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            CuentaPres data = new CuentaPres();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                var fromDb = DB.t_cuenta_pres.Where(c => c.id_cuent == id).FirstOrDefault();
                if (fromDb == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult() { Success = false, Data = "Not found" });
                }
                var pres = DB.t_presupuesto.Where(a => a.id_pres == fromDb.id_pres).FirstOrDefault();
                if (DB.t_Gastos.Any(a => (a.fecha >= pres.Desde && a.fecha <= pres.Hasta) && a.id_cat == fromDb.id_cat))
                {
                    return Json(new ResponseResult() { Success = false, Data = "No se puede eliminar la cuenta, debido a que tiene registros vinculados" });
                }

                DB.t_cuenta_pres.Remove(fromDb);
                DB.SaveChanges();

            }
            return Json(new ResponseResult() { Success = true, Data = "Éxito" });
        }
        [HttpGet]
        public string GetAll(int? id)
        {
            List<CuentaPres> data = new List<CuentaPres>();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                data.AddRange((from c in DB.t_cuenta_pres
                               join cat in DB.t_Categoria_gasto on c.id_cat equals cat.id_cat
                               where c.id_pres == id || id == null
                               select new CuentaPres()
                               {
                                   id_cuent = c.id_cuent,
                                   id_pres = c.id_pres,
                                   id_cat = c.id_cat,
                                   Limite = c.Limite,
                                   Categoria = cat.Nombre
                               }).ToList());
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
    }
}