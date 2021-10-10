using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokemonCRUD.Core.Common;
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
        #region Constructor & Properties
        private readonly IPokemonService _pokemonService;
        private readonly ILogger _logger;

        public PokemonController(IPokemonService pokemonService, ILogger<PokemonController> logger)
        {
            _pokemonService = pokemonService;
            _logger = logger;
        }

        #endregion

        #region Secure EndPoints

        /// <summary>
        /// Secured with authorize method. Returns the specified Pokemon by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Pokemon</returns>
        [HttpGet("name/secure")]
        [Authorize]
        public ActionResult<Pokemon> SecuredPokemonDetail(string name)
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
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        ///  Returns the of Pokemons specific for a Page separated by a Number of Rows Per Page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="numberRowsPerPage"></param>
        /// <returns>List<Pokemon></returns>
        [HttpGet("paginated/secure")]
        [Authorize]
        public ActionResult<IList<Pokemon>> SecurePokemonPaginated([Required] int pageNumber, [Required] int numberRowsPerPage)
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
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Creates a new Pokemon, if the Pokemon already exists returns no content
        /// </summary>
        /// <param name="pokemon"></param>
        /// <returns></returns>
        [HttpPost("new/secure")]
        [Authorize]
        public ActionResult<Pokemon> SecureNewPokemon([FromBody] Pokemon pokemon)
        {
            try
            {
                _logger.LogInformation("Starting the creation of a new {Pokemon}", pokemon);
                var result = _pokemonService.AddPokemon(pokemon);

                return result switch
                {
                    "Ok" => StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemon.Name} created"),
                    "Exists" => StatusCode(StatusCodes.Status204NoContent, $"Pokemon {pokemon.Name} already exists"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, $"Error creating the {pokemon}")
                };
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Modifies a Pokemon based on the originalName
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <param name="pokemon"></param>
        /// <returns>Ok(200) if updated correctly</returns>
        [HttpPut("modify/{pokemonName}/secure")]
        [Authorize]
        public ActionResult<Pokemon> SecureUpdatePokemon(string pokemonName, [FromBody] Pokemon pokemon)
        {
            try
            {
                _logger.LogInformation("Starting the modification of {PokemonName}", pokemonName);
                var result = _pokemonService.ModifyPokemon(pokemonName, pokemon);

                return result switch
                {
                    ResultMessage.Updated => StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemonName} updated"),
                    ResultMessage.Exists => StatusCode(StatusCodes.Status208AlreadyReported, $"Pokemon {pokemonName} already exists"),
                    ResultMessage.NotFound => StatusCode(StatusCodes.Status404NotFound, $"Pokemon {pokemonName} was not found"),
                    ResultMessage.Empty => StatusCode(StatusCodes.Status204NoContent, $"CSV File was empty"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, $"Error updating the Pokemon"),
                };
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a Pokemon based on the pokemonName
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns>Ok(200) if deleted correctly</returns>
        [HttpDelete("delete/{pokemonName}/secure")]
        [Authorize]
        public ActionResult<Pokemon> SecureDeletePokemon(string pokemonName)
        {
            try
            {
                _logger.LogInformation("Starting the creation of a new {Pokemon}", pokemonName);
                var result = _pokemonService.DeletePokemon(pokemonName);

                return result switch
                {
                    ResultMessage.Deleted => StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemonName} deleted"),
                    ResultMessage.NotFound => StatusCode(StatusCodes.Status404NotFound, $"Pokemon {pokemonName} was not found"),
                    ResultMessage.Empty => StatusCode(StatusCodes.Status204NoContent, $"CSV File was empty"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting the Pokemon"),
                };
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        #endregion

        #region EndPoints
        /// <summary>
        /// Returns the specified Pokemon by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Pokemon</returns>
        [HttpGet("name")]
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
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        /// <summary>
        ///  Returns the of Pokemons specific for a Page separated by a Number of Rows Per Page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="numberRowsPerPage"></param>
        /// <returns>List<Pokemon></returns>
        [HttpGet("paginated")]
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
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Creates a new Pokemon, if the Pokemon already exists returns no content
        /// </summary>
        /// <param name="pokemon"></param>
        /// <returns></returns>
        [HttpPost("new")]
        public ActionResult<Pokemon> NewPokemon([FromBody] Pokemon pokemon)
        {
            try
            {
                _logger.LogInformation("Starting the creation of a new {Pokemon}", pokemon);
                var result = _pokemonService.AddPokemon(pokemon);

                return result switch
                {
                    "Ok" => StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemon.Name} created"),
                    "Exists" => StatusCode(StatusCodes.Status204NoContent, $"Pokemon {pokemon.Name} already exists"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, $"Error creating the {pokemon}")
                };
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        /// <summary>
        /// Modifies a Pokemon based on the originalName
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <param name="pokemon"></param>
        /// <returns>Ok(200) if updated correctly</returns>
        [HttpPut("modify/{pokemonName}")]
        public ActionResult<Pokemon> UpdatePokemon(string pokemonName, [FromBody] Pokemon pokemon)
        {
            try
            {
                _logger.LogInformation("Starting the modification of {PokemonName}", pokemonName);
                var result = _pokemonService.ModifyPokemon(pokemonName, pokemon);

                return result switch
                {
                    ResultMessage.Updated => StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemonName} updated"),
                    ResultMessage.Exists => StatusCode(StatusCodes.Status208AlreadyReported, $"Pokemon {pokemonName} already exists"),
                    ResultMessage.NotFound => StatusCode(StatusCodes.Status404NotFound, $"Pokemon {pokemonName} was not found"),
                    ResultMessage.Empty => StatusCode(StatusCodes.Status204NoContent, $"CSV File was empty"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, $"Error updating the Pokemon"),
                };
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        /// <summary>
        /// Deletes a Pokemon based on the pokemonName
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns>Ok(200) if deleted correctly</returns>
        [HttpDelete("delete/{pokemonName}")]
        public ActionResult<Pokemon> DeletePokemon(string pokemonName)
        {
            try
            {
                _logger.LogInformation("Starting the creation of a new {Pokemon}", pokemonName);
                var result = _pokemonService.DeletePokemon(pokemonName);

                return result switch
                {
                    ResultMessage.Deleted => StatusCode(StatusCodes.Status200OK, $"Pokemon {pokemonName} deleted"),
                    ResultMessage.NotFound => StatusCode(StatusCodes.Status404NotFound, $"Pokemon {pokemonName} was not found"),
                    ResultMessage.Empty => StatusCode(StatusCodes.Status204NoContent, $"CSV File was empty"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting the Pokemon"),
                };
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status404NotFound, "The CSV file was not found");
            }
            catch (Exception ex)
            {
                _logger.LogError("{@exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion
    }
}
