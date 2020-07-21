using System;
using System.ComponentModel.DataAnnotations;
using DataSystem.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class MamAttribute : ValidationAttribute, IClientModelValidator
{
    public void AddValidation(ClientModelValidationContext context)
    {
        if(context==null){
        throw new NotImplementedException();
        }
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        TblMam model = (TblMam)validationContext.ObjectInstance;

        if (model.Zscore23+model.Muac23+model.Muac12+model.ReferIn+model.Totalbegin  < model.Cured+model.Deaths+model.Defaulters+model.NonCured+model.Transfers )
        {
            return new ValidationResult("Total Admission is greater than total exit.");
        }
        if (model.Zscore23+model.Muac23+model.Muac12 != model.TMale+model.TFemale )
        {
            return new ValidationResult(" Odema+Z3score+Muac115 is not qual to sum of male and females.");
        }

        return ValidationResult.Success;
    }

}