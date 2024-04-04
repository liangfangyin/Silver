using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Silver.WatchDog.src.Filters;
using Silver.WatchDog.src.Helpers;
using Silver.WatchDog.src.Managers;
using Silver.WatchDog.src.Models;

namespace Silver.WatchDog.src.Controllers
{
    [AllowAnonymous]
    public class WatchPageController : ControllerBase
    {
        public WatchPageController()
        {

        }

        [CustomAuthenticationFilter]
        public async Task<IActionResult> Index(string searchString = "", string verbString = "", string statusCode = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber);
            return Ok(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data });
        }

        [CustomAuthenticationFilter]
        public async Task<IActionResult> Exceptions(string searchString = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllWatchExceptionLogs(searchString, pageNumber);
            return Ok(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data });
        }

        [CustomAuthenticationFilter]
        public async Task<IActionResult> Logs(string searchString = "", string logLevelString = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllLogs(searchString, logLevelString, pageNumber);
            return Ok(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data });
        }

        [CustomAuthenticationFilter]
        public async Task<IActionResult> ClearLogs()
        {
            var cleared = await DynamicDBManager.ClearLogs(); 
            return Ok(cleared);
        }


        [HttpPost]
        public IActionResult Auth(string username, string password)
        {

            if (username.ToLower() == WatchDogConfigModel.UserName.ToLower() && password == WatchDogConfigModel.Password)
            {
                HttpContext.Session.SetString("isAuth", "true");
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("isAuth");
            return Ok(true); 
        }

        public IActionResult IsAuth()
        {
            
            if (!HttpContext.Session.TryGetValue("isAuth", out var isAuth))
            {
                return Ok(false);
            }
            else
            {
                return Ok(true);
            }
        }
    }
}
