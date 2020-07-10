using FinanzasPersonales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanzasPersonales.Controllers
{
    public class PresupuestosController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(PresupuestoCreate model)
        {
            if (ModelState.IsValid)
            {
                //Save
                using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
                {
                    //Validando que la fecha desde sea mayor o igual a la fecha hasta
                    if (model.Desde > model.Hasta)
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                        new ValidationResult() {
                         Key = "id_pres",
                         Errors = new ModelErrorCollection() { "La fecha 'Hasta' debe ser igual o mayor a la fecha 'Desde'" }
                        }
                        }
                        });
                    }
                    //Validando que otro presupuesto no entre en conflicto de fechas con este
                    if (DB.t_presupuesto.Any(p =>
                        (p.Desde <= model.Desde
                        &&
                        p.Hasta >= model.Desde)
                        ||
                        (p.Desde <= model.Hasta
                        &&
                        p.Hasta >= model.Hasta))
                    )
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                        new ValidationResult() {
                         Key = "id_pres",
                         Errors = new ModelErrorCollection() { "Hay conflictos entre las fechas digitadas y las que existen" }
                        }
                        }
                        });
                    }
                    DB.t_presupuesto.Add(new Data.t_presupuesto()
                    {
                        Nombre = model.Nombre,
                        Desde = model.Desde,
                        Hasta = model.Hasta
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
        public JsonResult Edit(Presupuesto model)
        {
            if (ModelState.IsValid)
            {
                //Save
                using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
                {
                    //Validando que la fecha desde sea mayor o igual a la fecha hasta
                    if (model.Desde > model.Hasta)
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                        new ValidationResult() {
                         Key = "id_pres",
                         Errors = new ModelErrorCollection() { "La fecha 'Hasta' debe ser igual o mayor a la fecha 'Desde'" }
                        }
                        }
                        });
                    }
                    //Validando que otro presupuesto no entre en conflicto de fechas con este
                    if (DB.t_presupuesto.Any(p =>
                        (
                            (p.Desde <= model.Desde
                            &&
                            p.Hasta >= model.Desde)
                            ||
                            (p.Desde <= model.Hasta
                            &&
                            p.Hasta >= model.Hasta)
                        )
                        &&
                        (p.id_pres != model.id_pres)
                    )
                    )
                    {
                        return Json(new ResponseResult()
                        {
                            Success = false,
                            Data = new List<ValidationResult>() {
                        new ValidationResult() {
                         Key = "id_pres",
                         Errors = new ModelErrorCollection() { "Hay conflictos entre las fechas digitadas y las que existen" }
                        }
                        }
                        });
                    }
                    Data.t_presupuesto data = DB.t_presupuesto.Where(c => c.id_pres == model.id_pres).FirstOrDefault();

                    data.id_pres = model.id_pres;
                    data.Nombre = model.Nombre;
                    data.Desde = model.Desde;
                    data.Hasta = model.Hasta;

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
            Presupuesto data = new Presupuesto();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                var fromDbCat = DB.t_presupuesto.Where(c => c.id_pres == id).FirstOrDefault();
                if (fromDbCat == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult() { Success = false, Data = "Not found" });
                }
                else
                {
                    data = new Presupuesto()
                    {
                        id_pres = fromDbCat.id_pres,
                        Nombre = fromDbCat.Nombre,
                        Desde = fromDbCat.Desde,
                        Hasta = fromDbCat.Hasta
                    };
                }
            }
            return Json(data);
        }
        [HttpPost]
        public JsonResult GetCurrent()
        {
            Presupuesto data = new Presupuesto();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                var date = DateTime.Now;
                //var date = new DateTime(2020,12,30);
                var fromDbCat = DB.t_presupuesto.Where(p => p.Desde <= date && p.Hasta >= date).FirstOrDefault();
                if (fromDbCat == null)
                {
                    return Json(new ResponseResult() { Success = false, Data = "Not found" });
                }
                else
                {
                    data = new Presupuesto()
                    {
                        id_pres = fromDbCat.id_pres,
                        Nombre = fromDbCat.Nombre,
                        Desde = fromDbCat.Desde,
                        Hasta = fromDbCat.Hasta
                    };
                }
            }
            return Json(new ResponseResult() { Success = true, Data = data });
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                //Validando no hay registros relacionados al presupuesto
                if (DB.t_cuenta_pres.Any(c => c.id_pres == id))
                {
                    return Json(new ResponseResult() { Success = false, Data = "No se puede eliminar el presupuesto, debido a que tiene registros vinculados" });
                }
                var fromDbCat = DB.t_presupuesto.Where(c => c.id_pres == id).FirstOrDefault();
                if (fromDbCat == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult() { Success = false, Data = "Not found" });
                }
                else
                {
                    DB.t_presupuesto.Remove(fromDbCat);
                    DB.SaveChanges();
                }
            }
            return Json(new ResponseResult() { Success = true, Data = "Éxito al eliminar" });
        }
        [HttpGet]
        public string GetAll()
        {
            List<Presupuesto> data = new List<Presupuesto>();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                data.AddRange(from p in DB.t_presupuesto
                              select new Presupuesto()
                              {
                                  id_pres = p.id_pres,
                                  Nombre = p.Nombre,
                                  Desde = p.Desde,
                                  Hasta = p.Hasta
                              });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(data.OrderBy(p => p.Desde));
        }
    }
}