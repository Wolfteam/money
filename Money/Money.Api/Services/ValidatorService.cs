using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Money.Api.Interfaces;

namespace Money.Api.Services
{
    public class ValidatorService : IValidatorService
    {
        protected readonly ILogger<ValidatorService> Logger;
        private readonly IValidatorFactory _validatorFactory;

        public ValidatorService(ILogger<ValidatorService> logger, IValidatorFactory validatorFactory)
        {
            Logger = logger;
            _validatorFactory = validatorFactory;
        }

        private IValidator GetValidator(Type dtoType)
        {
            return _validatorFactory.GetValidator(dtoType);
        }

        public void ValidateDto<TRequest>(TRequest dto, [CallerMemberName] string callerMember = "")
        {
            Logger.LogInformation($"{callerMember}: Validating request dto = {{@dto}}", dto);
            var type = dto.GetType();
            var validator = GetValidator(type);
            if (validator is null)
            {
                Logger.LogWarning($"Cant validate dtoType = {type} because no validator was found");
                throw new Exception($"Did you forget to add a validator for = {type} ? .");
            }

            var context = new ValidationContext<TRequest>(dto);
            var result = validator.Validate(context);
            if (result.IsValid)
                return;

            var errors = result.Errors
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
            var errorKey = result.Errors.FirstOrDefault().ErrorCode;
            var ex = new Exceptions.ValidationException(errors, errorKey);

            Logger.LogWarning($"{callerMember}: A validation error occurred. Error = {ex.Error}");
            throw ex;
        }
    }

}
