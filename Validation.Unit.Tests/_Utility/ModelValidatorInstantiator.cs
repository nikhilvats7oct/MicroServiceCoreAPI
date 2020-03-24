using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Validation.Unit.Tests._Utility
{
    static class ModelValidatorInstantiator
    {
        internal static IValidator<T> GetValidatorFromModelAttribute<T>()
        {
            ValidatorAttribute validatorAttribute = (ValidatorAttribute)typeof(T).GetCustomAttribute(typeof(ValidatorAttribute));
            Assert.IsNotNull(validatorAttribute, "Model have validator");

            IValidator<T> validator = (IValidator<T>)Activator.CreateInstance(validatorAttribute.ValidatorType);

            return validator;
        }
    }
}
