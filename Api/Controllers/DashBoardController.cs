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
    public class DashBoardController : BaseController
    {
        private readonly IDashboardManager _dashboardManager;

        public DashBoardController(IDashboardManager dashboardManager)
        {
            _dashboardManager = dashboardManager;
        }
        [Authorize]
        [HttpGet("DashBoard")]
        public async Task<ActionResult> DashBoard()
        {
            var result = await _dashboardManager.GetDashBoardStatAsync(LogedInUserId);
            return GetResponse(result);
        }
    }
}
