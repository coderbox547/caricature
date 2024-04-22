using Data.Contract.Response;
using Data.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Api.Helpers.IdentityException;

namespace Api.Controllers
{

    public class BaseController : Controller
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult Success(object data = null)
        {
            return Ok(data);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult Success(string message = null)
        {
            return Ok(new { Status = true, Message = message });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult Success(string message, object data)
        {
            return Ok(new
            {
                Message = message,
                Data = data
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult BadRequest(string message = null)
        {
            return Ok(new { Status = false, Message = message });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult GetResponse(SaveOrUpdateOrDeleteResponse res)
        {
            if (res.IsErrorOccured)
            {
                ErrorType errorType = res.Errors.FirstOrDefault().ErrorType;

                switch (errorType)
                {
                    case ErrorType.Argument:
                        break;

                    case ErrorType.Informative:
                        throw new InformativeException(res.GetErrorsWithCommaSeperate);

                    case ErrorType.Exception:
                        throw new Exception(res.GetErrorsWithCommaSeperate);

                    default:
                        break;
                }
            }

            return Success(res);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult GetResponse<T>(SaveOrUpdateOrDeleteResponse<T> res)
        {
            if (res.IsErrorOccured)
            {
                ErrorType errorType = res.Errors.FirstOrDefault().ErrorType;

                switch (errorType)
                {
                    case ErrorType.Argument:
                        break;

                    case ErrorType.Informative:
                        throw new InformativeException(res.GetErrorsWithCommaSeperate);

                    case ErrorType.Exception:
                        throw new Exception(res.GetErrorsWithCommaSeperate);

                    default:
                        break;
                }
            }

            return Success(res);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult GetResponse<T>(SearchResponse<T> res)
        {
            if (res.IsErrorOccured)
            {
                ErrorType errorType = res.Errors.FirstOrDefault().ErrorType;

                switch (errorType)
                {
                    case ErrorType.Argument:
                        break;

                    case ErrorType.Informative:
                        throw new InformativeException(res.GetErrorsWithCommaSeperate);

                    case ErrorType.Exception:
                        throw new Exception(res.GetErrorsWithCommaSeperate);

                    default:
                        break;
                }
            }

            return Success(res);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult GetResponse<T>(GetResponse<T> res)
        {
            if (res.IsErrorOccured)
            {
                ErrorType errorType = res.Errors.FirstOrDefault().ErrorType;

                switch (errorType)
                {
                    case ErrorType.Argument:
                        break;

                    case ErrorType.Informative:
                        throw new InformativeException(res.GetErrorsWithCommaSeperate);

                    case ErrorType.Exception:
                        throw new Exception(res.GetErrorsWithCommaSeperate);

                    default:
                        break;
                }
            }

            return Success(res);
        }
      
        public static bool IsUserAuthenticated(HttpContext httpContext)
        {
            return httpContext.User.Identity.IsAuthenticated;
        }

        public string LogedInUserId
        {
            get
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst("id");
                return claim.Value;
            }
        }
    }
}
