using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokemonCRUD.Core.Common;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public long CountLinesInFile()
        {
            return File.ReadLines(_csvPath)
                .Where(p => p != null)
                .LongCount();
        }

        public Pokemon GetByName(string name)
        {
            var pokemon = File.ReadLines(_csvPath)
                .Skip(1) //Skip the first line that contains the headers
                .Select(ParsePokemonFromLine)
                .Where(p => p.Name.ToLower() == name.ToLower())
                .FirstOrDefault();
            return pokemon;
        }

        public List<Pokemon> GetAllPaginated(int startingRow, int numberRowsPerPage)
        {
            var pokemons = File.ReadLines(_csvPath)
                .Skip(startingRow)
                .Select(ParsePokemonFromLine)
                .Take(numberRowsPerPage)
                .ToList();

            return pokemons;
        }

        public Pokemon AddNew(Pokemon pokemon)
        {
            var line = new StringBuilder();
            line.Append(FormatPokemonToCsv(pokemon));
            File.AppendAllText(_csvPath, line.ToString());
            return pokemon;
        }

        public string Modify(string originalName, Pokemon pokemon)
        {
            return UpdateDeletePokemon(originalName, pokemon, false);
        }

        public string Delete(string name)
        {
            return UpdateDeletePokemon(name, null, true);
        }

        #region Private Methods
        private bool VerifyIfPokemonExists(string name)
        {
            return File.ReadLines(_csvPath)
                   .Skip(1) //Skip the first line that contains the headers
                   .Select(ParsePokemonFromLine)
                   .Where(p => p.Name.ToLower() == name.ToLower())
                   .Any();
        }
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
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}{13}"
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

        private string UpdateDeletePokemon(string pokemonOriginalName, Pokemon pokemon, bool deleteRecord)
        {
            var delimiter = ",";
            var tempPath = Path.GetTempFileName();
            string result = ResultMessage.NotFound;
            using (var writer = new StreamWriter(tempPath))
            using (var reader = new StreamReader(_csvPath))
            {
                //Read the Header and Write it
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    return ResultMessage.Empty; // File is empty
                }

                writer.WriteLine(line);

                while ((line = reader.ReadLine()) != null)
                {
                    var columns = line.Split(',').Where(s => s != delimiter).ToArray();

                    //Column[1] is the name and the unique identifier
                    if (columns[1].ToLower().Equals(pokemonOriginalName.ToLower()))
                    {
                        if (deleteRecord)
                        {
                            result = ResultMessage.Deleted;
                            continue;
                        }
                        
                        writer.Write(FormatPokemonToCsv(pokemon));
                        result = ResultMessage.Updated;

                        continue;
                    }

                    writer.WriteLine(string.Join(delimiter, columns));
                }
            }

            File.Delete(_csvPath);
            File.Move(tempPath, _csvPath);

            return result;
        }
        #endregion
    }
}
