using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PokemonCRUD.Services
{
    public class PokemonService : IPokemonService
    {
        #region Constructor & Properties
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
        #endregion

        #region Public Methods

        public Pokemon GetPokemonByName(string name)
        {
            CheckFileExists();

            _logger.LogInformation("Starting the search of Pokemon by name: {name}", name);
            var pokemon = _pokemonRepository.GetByName(name);
            return pokemon;
        }

        public List<Pokemon> GetPokemonPaginated(int pageNumber, int numberRowsPerPage)
        {
            CheckFileExists();

            var count = (decimal)_pokemonRepository.CountLinesInFile();
            var aux = count / numberRowsPerPage;
            var numberOfPages = Math.Ceiling(aux);

            if (pageNumber > numberOfPages)
            {
                _logger.LogWarning("The {pageNumber} asked is greater than the total number of pages", pageNumber);
                return null;
            }

            var startingRow = pageNumber * numberRowsPerPage;

            //Get the starting row by substracting the number of rows per pages. Add 1 row to take the next page
            startingRow = startingRow - numberRowsPerPage + 1;

            var pokemons = _pokemonRepository.GetAllPaginated(startingRow, numberRowsPerPage);
            return pokemons;
        }

        public string AddPokemon(Pokemon newPokemon)
        {
            CheckFileExists();            
            var result = _pokemonRepository.AddNew(newPokemon);
            return result;
        }

        public string ModifyPokemon(string orignalName, Pokemon newPokemon)
        {
            CheckFileExists();
            var result = _pokemonRepository.Modify(orignalName, newPokemon);
            return result;
        }

        public string DeletePokemon(string orignalName)
        {
            CheckFileExists();
            var result = _pokemonRepository.Delete(orignalName);
            return result;
        }

        #endregion

        #region Private Methods
        private void CheckFileExists()
        {
            if (!File.Exists(_csvPath))
            {
                throw new FileNotFoundException();
            }
        }
        #endregion
    }
}
