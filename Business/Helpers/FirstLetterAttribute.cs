﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebApiDemo.Business.Helpers
{
    public class FirstLetterAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                var firstLetter = value.ToString()[0].ToString();

                if (firstLetter != firstLetter.ToUpper())
                {
                    return new ValidationResult("First letter must be capitalized");
                }
            }

            return ValidationResult.Success;
        }
    }
}
