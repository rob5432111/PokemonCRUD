using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace PokemonCRUD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : Controller
    {
        private readonly IPokemonService _pokemonService;
        private readonly ILogger _logger;

        public PokemonController(IPokemonService pokemonService, ILogger<PokemonController> logger)
        {
            _pokemonService = pokemonService;
            _logger = logger;
        }

        /// <summary>
        /// Returns the specified Pokemon by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Pokemon</returns>
        [HttpGet("/name")]
        public ActionResult<Pokemon> PokemonDetail(string name)
        {
            try
            {
                _logger.LogInformation("Starting the process of Pokemon search for: {name}", name);
                var pokemon = _pokemonService.GetPokemonByName(name);
                return pokemon;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
        }

        /// <summary>
        ///  Returns the of Pokemons specific for a Page separated by a Number of Rows Per Page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="numberRowsPerPage"></param>
        /// <returns>List<Pokemon></returns>
        [HttpGet("/paginated")]
        public ActionResult<IList<Pokemon>> PokemonPaginated([Required] int pageNumber, [Required] int numberRowsPerPage)
        {
            try
            {
                _logger.LogInformation("Starting the process of Pokemon Detail Paginated for: {pageNumber}", pageNumber, numberRowsPerPage);
                var pokemons = _pokemonService.GetPokemonPaginated(pageNumber, numberRowsPerPage);
                return pokemons;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
        }

        [HttpPost("/new")]
        public ActionResult<Pokemon> NewPokemon([FromBody] Pokemon pokemon)
        {
            try
            {
                _logger.LogInformation("Starting the creation of a new {Pokemon}", pokemon);
                var pokemons = _pokemonService.AddPokemon(pokemon);
                return pokemons;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
        }
    }
}
