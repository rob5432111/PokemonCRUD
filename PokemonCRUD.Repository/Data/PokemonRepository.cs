using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PokemonCRUD.Repository.Data
{

    public class PokemonRepository : IPokemonRepository
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<PokemonRepository> _logger;
        private readonly string _csvPath;

        public PokemonRepository(IOptions<AppSettings> options, ILogger<PokemonRepository> logger)
        {            
            _appSettings = options.Value;
            _logger = logger;
            _csvPath = _appSettings.ApplicationConfiguration.CsvPath;
        }

        /// <summary>
        /// Returns the number of lines in the Pokemon CSV
        /// </summary>        
        /// <returns>The number of lines in the CSV</returns>
        public long CountLinesInFile()
        {
            return File.ReadLines(_csvPath)
                .Where(p => p != null)
                .LongCount();
        }

        public Pokemon GetByName(string name)
        {          
            try
            {
                var pokemon = File.ReadLines(_csvPath)
                    .Skip(1) //Skip the first line that contains the headers
                    .Select(ParsePokemonFromLine)
                    .Where(p => p.Name == name)
                    .FirstOrDefault();
                return pokemon;

            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return null;
            }           
        }

        public List<Pokemon> GetAllPaginated(int startingRow, int numberRowsPerPage)
        {           

            try
            {
                var pokemons = File.ReadLines(_csvPath)
                    .Skip(startingRow)                    
                    .Select(ParsePokemonFromLine)
                    .Take(numberRowsPerPage)
                    .ToList();
                    
                return pokemons;

            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return null;
            }

        }

        public string AddNew(Pokemon pokemon)
        {
            try
            {
                var csv = new StringBuilder();
                csv.Append(FormatPokemonToCsv(pokemon));
                File.WriteAllText(_csvPath, csv.ToString());
                return "Ok";
            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return ex.Message;
            }
        }

        #region Private Methods
        private static Pokemon ParsePokemonFromLine(string line)
        {
            string[] parts = line.Split(',');
            return new Pokemon
            {
                Number = int.Parse(parts[0]),
                Name = parts[1],
                Type1 = parts[2],
                Type2 = parts[3],
                Total = int.Parse(parts[4]),
                HP = int.Parse(parts[5]),
                Attack = int.Parse(parts[6]),
                Defense = int.Parse(parts[7]),
                SpAttack = int.Parse(parts[8]),
                SpDefense = int.Parse(parts[9]),
                Speed = int.Parse(parts[10]),
                Generation = int.Parse(parts[11]),
                Legendary = bool.Parse(parts[12])
            };
        }

        private static string FormatPokemonToCsv(Pokemon pokemon)
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}"
                , pokemon.Number //0
                , pokemon.Name //1
                , pokemon.Type1 //2
                , pokemon.Type2 //3
                , pokemon.Total //4
                , pokemon.HP //5
                , pokemon.Attack //6
                , pokemon.Defense //7
                , pokemon.SpAttack //8
                , pokemon.SpDefense //9
                , pokemon.Speed //10
                , pokemon.Generation //11
                , pokemon.Legendary //12
                , Environment.NewLine);
        }
        #endregion
    }
}
