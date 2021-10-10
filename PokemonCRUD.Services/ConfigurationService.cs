using PokemonCRUD.Core.Interfaces;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokemonCRUD.Core.Models;
using Microsoft.AspNetCore.Http;

namespace PokemonCRUD.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public readonly ILogger<ConfigurationService> _logger;
        private readonly AppSettings _appSettings;
        public ConfigurationService(IOptions<AppSettings> config, ILogger<ConfigurationService> logger)
        {
            _appSettings = config.Value;
            _logger = logger;
        }
        public string ConfigureCsvPath(string csvPath)
        {     

            bool fileExists = File.Exists(csvPath);
            if (!fileExists)
            {
                _logger.LogInformation("The file specified doesn't exists {File.Exists} {csvPath}", fileExists, csvPath);
                return "NotExists";
            }

            _appSettings.ApplicationConfiguration.CsvPath = csvPath;

            _logger.LogInformation("Csv File Path configured correctly to: {csvPath}", csvPath);
            return "Ok";
            
        }
    }
}
