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

        /// <summary>
        /// Creates a new Pokemon, if the Pokemon already exists returns no content
        /// </summary>
        /// <param name="pokemon"></param>
        /// <returns></returns>
        [HttpPost("/new")]
        public ActionResult<Pokemon> NewPokemon([FromBody] Pokemon pokemon)
        {
            try
            {
                _logger.LogInformation("Starting the creation of a new {Pokemon}", pokemon);
                var result = _pokemonService.AddPokemon(pokemon);
                if (result.Equals("Ok"))
                {
                    return StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemon.Name} created");
                }
                else if (result.Equals("Exists"))
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"Pokemon {pokemon.Name} already exists");
                }

                return StatusCode(StatusCodes.Status204NoContent, $"Error creating the new Pokemon");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
        }


        [HttpPut("/modify/{pokemonName}")]
        public ActionResult<Pokemon> UpdatePokemon(string pokemonName, [FromBody] Pokemon pokemon)
        {
            try
            {
                _logger.LogInformation("Starting the creation of a new {Pokemon}", pokemon);
                var result = _pokemonService.ModifyPokemon(pokemonName, pokemon);
                if (result.Equals("Updated"))
                {
                    return StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemonName} updated");
                }
                else if (result.Equals("Empty"))
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"CSV File was empty");
                }
                else if (result.Equals("Exists"))
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"Pokemon {pokemonName} already exists");
                }
                else if (result.Equals("NotFound"))
                {
                    return StatusCode(StatusCodes.Status404NotFound, $"Pokemon {pokemonName} was not found");
                }

                return StatusCode(StatusCodes.Status204NoContent, $"Error updating the Pokemon");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
        }

        [HttpDelete("/delete/{pokemonName}")]
        public ActionResult<Pokemon> DeletePokemon(string pokemonName)
        {
            try
            {
                _logger.LogInformation("Starting the creation of a new {Pokemon}", pokemonName);
                var result = _pokemonService.DeletePokemon(pokemonName);
                if (result.Equals("Deleted"))
                {
                    return StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemonName} deleted");
                }
                else if (result.Equals("Empty"))
                {
                    return StatusCode(StatusCodes.Status204NoContent, $"CSV File was empty");
                }
                else if (result.Equals("NotFound"))
                {
                    return StatusCode(StatusCodes.Status404NotFound, $"Pokemon {pokemonName} was not found");
                }


                return StatusCode(StatusCodes.Status204NoContent, $"Error deleting the Pokemon");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
        }
    }
}
