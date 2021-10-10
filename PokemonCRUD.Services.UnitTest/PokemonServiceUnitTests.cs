using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using System.IO.Abstractions;
using Xunit;

namespace PokemonCRUD.Services.UnitTest
{
    public class PokemonServiceUnitTests
    {
        
        [Fact]
        //[MemberData(nameof(Data))]
        //public void GetPokemonByName_PokemonName_ReturnsPokemon(string pokemonName, Pokemon expectedPokemon)
        public void GetPokemonByName_PokemonName_ReturnsPokemon()
        {
            // Arrange
            var fakePokemon = new Pokemon
            {
                Number = 1,
                Name = "Pikachu"
            };

            var pokemonName = "Pikachu";

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Options.Create(new AppSettings {});
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();
             
            pokemonRepository.GetByName(Arg.Is(pokemonName)).Returns(fakePokemon);
            fileSystem.File.Exists(Arg.Any<string>()).Returns(true);
            

            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.GetPokemonByName(pokemonName);
            // Assert

            Assert.Equal(pokemonName, result.Name);
        }
    }
}
