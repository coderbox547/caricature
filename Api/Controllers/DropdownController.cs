using Data.Contract.Request;
using Data.Contract.Response;
using Data.Domain;
using Data.Managers;
using Data.Repository.EntityFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DropdownController : BaseController
    {
        private readonly IDropdownManager _dropdownManager;

        public DropdownController(IDropdownManager dropdownManager)
        {
            _dropdownManager = dropdownManager;
        }

        [HttpGet("BindDropdown1")]
        public async Task<IActionResult> BindDropdown1()
        {
            var result = await _dropdownManager.GetDropdown1();
            return GetResponse(result);
        }
    }
}
