using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokemonCRUD.Core.Common;
using PokemonCRUD.Core.Helper;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonCRUD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
       
        private readonly ILogger _logger;
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IOptions<AppSettings> config, ILogger<PokemonController> logger,
            IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _logger = logger;
        }

        [HttpPut("FilePath")]       
        public IActionResult FilePathConfiguration([Required] string csvPath)
        {
            try
            {
                _logger.LogInformation("Starting the FilePath Configuration process: {csvPath}", csvPath);

                //Call to the configuration service to set the path
                var result = _configurationService.ConfigureCsvPath(csvPath);

                if (result.Equals(ResultMessage.Ok))
                {
                    return StatusCode(StatusCodes.Status200OK, CustomMessage.OkCsvPath);
                }
                else if (result.Equals(ResultMessage.NotFound))
                {
                    return StatusCode(StatusCodes.Status404NotFound, CustomMessage.NotExistsFile);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, CustomMessage.ErrorFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{@ex}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("token")]
        public IActionResult GenerateJwtToken()
        {
            try
            {
                return new ObjectResult(JwtHelper.GenerateJwtToken());
            }
            catch (Exception ex)
            {
                _logger.LogError("{@ex}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
