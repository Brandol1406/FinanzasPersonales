using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanzasPersonales.Models
{
    public static class Validations
    {
        public static IEnumerable<ValidationResult> GetErrors(ModelStateDictionary state)
        {
            return state.Select(v => new ValidationResult() { Errors = v.Value.Errors, Key = v.Key });
        }
    }
    public class ValidationResult
    {
        public ModelErrorCollection Errors { get; set; }
        public string Key { get; set; }
    }
}