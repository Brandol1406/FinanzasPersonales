using FinanzasPersonales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanzasPersonales.Controllers
{
    public class CategoriasController : Controller
    {
        // Registro Gastos
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(CategoriaCreate model)
        {
            if (ModelState.IsValid)
            {
                //Save
                using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
                {
                    DB.t_Categoria_gasto.Add(new Data.t_Categoria_gasto()
                    {
                        Nombre = model.Nombre
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
        public JsonResult Edit(Categoria model)
        {
            if (ModelState.IsValid)
            {
                //Save
                using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
                {
                    Data.t_Categoria_gasto data = DB.t_Categoria_gasto.Where(c => c.id_cat == model.id_cat).FirstOrDefault();

                    data.id_cat = model.id_cat;
                    data.Nombre = model.Nombre;

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
           Categoria data = new Categoria();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                var fromDbCat = DB.t_Categoria_gasto.Where(c => c.id_cat == id).FirstOrDefault();
                if (fromDbCat == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult() { Success = false , Data = "Not found"});
                }
                else
                {
                    data = new Categoria() {
                        id_cat = fromDbCat.id_cat,
                        Nombre= fromDbCat.Nombre
                    };
                }
            }
            return Json(data);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                //Validando no hay registros relacionados a la categoria
                if (DB.t_cuenta_pres.Any(c => c.id_cat == id) || DB.t_Gastos.Any(g => g.id_cat == id))
                {
                    return Json(new ResponseResult() { Success = false, Data = "No se puede eliminar la categoría, debido a que tiene registros vinculados" });
                }
                var fromDbCat = DB.t_Categoria_gasto.Where(c => c.id_cat == id).FirstOrDefault();
                if (fromDbCat == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ResponseResult() { Success = false , Data = "Not found"});
                }
                else
                {
                    DB.t_Categoria_gasto.Remove(fromDbCat);
                    DB.SaveChanges();
                }
            }
            return Json(new ResponseResult() { Success = true, Data = "Éxito al eliminar" });
        }
        [HttpGet]
        public string GetAll()
        {
            List<Categoria> data = new List<Categoria>();
            using (Data.FinanzasPersonales DB = new Data.FinanzasPersonales())
            {
                data.AddRange(DB.t_Categoria_gasto.Select(c => new Categoria() { 
                    id_cat = c.id_cat,
                    Nombre = c.Nombre
                }).ToList());
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
    }
}