using System.Collections.Generic;

namespace PokemonCRUD.Core.Models
{
    public class AppSettings
    {
        public ApplicationConfiguration ApplicationConfiguration { get; set; }          
    }

    public class ApplicationConfiguration
    {
        public string CsvPath { get; set; }
    }

}