using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EVA.EIMS.AutoEmailerScheduler
{
    [Route("api/[controller]/[action]")]
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public class ValuesController : Controller
    {
        private HealthCheckService _healthCheckService;

        public ValuesController(HealthCheckService healthCheckService)
        {
            this._healthCheckService = healthCheckService;
        }

        [Route("/Healthz")]
        [HttpGet]
        public IActionResult Healthz()
        {
            return Ok("Healthy");
        }

        [Route("/GetHealth")]
        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            //// Can also call RunCheckAsync to run a single Heatlh Check or RunGroupAsync to run a group of Health Checks
            //var healthCheckResult = await _healthCheckService.CheckHealthAsync();

            //bool somethingIsWrong = healthCheckResult.Status != HealthStatus.Healthy;

            //if (somethingIsWrong)
            //{
            //    // healthCheckResult has a .Description property, but that shows the description of all health checks. 
            //    // Including the successful ones, so let's filter those out
            //    var failedHealthCheckDescriptions = healthCheckResult.Entries.Where(r => r.Value.Status != HealthStatus.Healthy)
            //                                                         .Select(r => string.Concat(r.Value.Description, r.Value.Exception))
            //                                                         .ToList();

            //    // return a 500 with JSON containing the Results of the Health Check
            //    return new JsonResult(new { Errors = failedHealthCheckDescriptions }) { StatusCode = 500 };
            //}

            return Ok("Healthy");
        }
    }
}
