using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PokemonCRUD.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly AppSettings _appSettings;
         private readonly string _csvPath;

        private readonly ILogger<PokemonService> _logger;
        public PokemonService(IOptions<AppSettings> options, IPokemonRepository pokemonRepository
            , ILogger<PokemonService> logger)
        {
            _appSettings = options.Value;
            _pokemonRepository = pokemonRepository;
            _csvPath = _appSettings.ApplicationConfiguration.CsvPath;
            _logger = logger;
        }

        public Pokemon GetPokemonByName(string name)
        {
            if (File.Exists(_csvPath))
            {
                _logger.LogInformation("Starting the search of Pokemon by name: {name}", name);
            var pokemon = _pokemonRepository.GetByName(name);
            return pokemon;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public List<Pokemon> GetPokemonPaginated(int pageNumber, int numberRowsPerPage)
        {
            if (File.Exists(_csvPath))
            {
                var count = (decimal)_pokemonRepository.CountLinesInFile();
                var aux = count / numberRowsPerPage;
                var numberOfPages = Math.Ceiling(aux);

                if (pageNumber > numberOfPages)
                {
                    _logger.LogWarning("The {pageNumber} asked is greater than the total number of pages", pageNumber);
                    return null;
                }    
                
                var startingRow = pageNumber * numberRowsPerPage;

                //Get the starting row by substracting the number of pages. Add 1 row to take the next page
                startingRow = startingRow - numberRowsPerPage + 1; 

                var pokemons = _pokemonRepository.GetAllPaginated(startingRow, numberRowsPerPage);
                return pokemons;
            }
            else 
            {
                throw new FileNotFoundException();
            }
        }

        public Pokemon AddPokemon(Pokemon newPokemon)
        {
            if (File.Exists(_csvPath))
            {
                var pokemonCreated = _pokemonRepository.AddNew(newPokemon);

                return new Pokemon { Name = "test" };
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
