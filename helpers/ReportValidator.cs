using System;
using System.ComponentModel.DataAnnotations;
using DataSystem.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class ReportValidatorAttribute : ValidationAttribute, IClientModelValidator
{
    public void AddValidation(ClientModelValidationContext context)
    {
        if(context==null){
        throw new NotImplementedException();
        }
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        TblOtptfu model = (TblOtptfu)validationContext.ObjectInstance;

        if (model.Cured+model.Death+model.Default+model.NonCured+model.RefOut> model.Odema+model.Z3score+model.Muac115+model.Fromscotp+model.Fromsfp+model.Defaultreturn+model.Totalbegin )
        {
            return new ValidationResult("Total Admission is greater than total exit.");
        }
        if (model.Odema+model.Z3score+model.Muac115  != model.TMale+model.TFemale)
        {
            return new ValidationResult(" Odema+Z3score+Muac115 is not qual to sum of male and females.");
        }

        return ValidationResult.Success;
    }

}