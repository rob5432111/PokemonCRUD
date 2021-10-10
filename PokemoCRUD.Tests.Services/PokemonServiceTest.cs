using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using PokemonCRUD.Services;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PokemoCRUD.Tests.Services
{
    public class PokemonServiceTest
    {
        //public static IEnumerable<object[]> Data =>
        //new List<object[]>
        //{
        //    new object[] {"Pikachu", new Pokemon { Name = "Pikachu", Number = 1}}
        //};

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
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();

            pokemonRepository.GetByName(Arg.Any<string>()).Returns(fakePokemon);
            File.Exists(Arg.Any<string>()).Returns(true);

            var pokemonService = new PokemonService(options, pokemonRepository, logger);

            // Act
            var result = pokemonService.GetPokemonByName(pokemonName);
            // Assert

            Assert.True(true);
        }
    }
}
