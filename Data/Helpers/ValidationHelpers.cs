using Data.Contract.Request;
using Data.Enums;
using Data.Repository.EntityFilters;
using System;
using System.Collections.Generic;

using ValidationEum = Data.Enums.ValidationsType;

namespace Data.Helpers
{
    public static class ValidationHelper
    {
        public static void Validate(int? entityId, ValidationEum validationEum)
        {
            List<string> errors = new List<string>();

            switch (validationEum)
            {
                case ValidationEum.EntityId:
                    if (entityId.HasValue == false)
                    {
                        errors.Add("EntityId nither can be 0 nor null");
                    }
                    break;


                default:
                    throw new ArgumentException("Invalid validation type");
            }

            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(",", errors));
            }

        }
        public static void Validate<TModel>(TModel model, Func<TModel, List<string>> validationLogic)
        {
            List<string> errors = validationLogic(model);

            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(",", errors));
            }
        }
    }
}
