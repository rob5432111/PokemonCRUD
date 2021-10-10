using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PokemonCRUD.Core.Common;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Abstractions;
using Xunit;

namespace PokemonCRUD.Services.UnitTest
{
    public class PokemonServiceUnitTests
    {
        #region GetPokemon Tests
        public static IEnumerable<object[]> GetPokemonDetail =>
        new List<object[]>
        {
            new object[] { "Pikachu", new Pokemon {  Name = "Pikachu", Number = 25} },
            new object[] { "NotAPokemon", null}
        };

        [Theory]
        [MemberData(nameof(GetPokemonDetail))]
        public void GetPokemonByName_PokemonName_ReturnsPokemon(string pokemonName, Pokemon expectedPokemon)
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var mockPokemon = new Pokemon
            {
                Name = "Pikachu",
                Number = 25
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.GetByName(Arg.Is(mockPokemon.Name)).Returns(mockPokemon);

            options.Value.Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>()).Returns(true);

            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.GetPokemonByName(pokemonName);

            // Assert            
            pokemonRepository
                .Received(1)
                .GetByName(
                    Arg.Is<string>(t => t == pokemonName));

            if (result != null)
            {
                Assert.Equal(expectedPokemon.Name, result.Name);
            }
        }
        #endregion

        #region GetPokemonPaginated Tests

        [Fact, Description("Test the get pokemons paginated, should return the page specified")]
        public void GetPokemonPaginated_PageNumberAndRowsPerPage_ReturnsListOfPokemons()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var mockPokemon = new List<Pokemon>
            {
                new Pokemon
                {
                    Name = "Pikachu",
                    Number = 25
                },
                new Pokemon
                {
                    Name = "Charmander",
                    Number = 4
                }
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.CountLinesInFile()
                .Returns(mockPokemon.Count);
            pokemonRepository.GetAllPaginated(Arg.Any<int>(), Arg.Any<int>())
                .Returns(mockPokemon);
            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            var pageNumber = 1;
            var numberRowsPerPage = 2;

            // Act
            var result = pokemonService.GetPokemonPaginated(pageNumber, numberRowsPerPage);

            // Assert           

            pokemonRepository
                .Received(1)
                .GetAllPaginated(
                    Arg.Is<int>(t => t == pageNumber), Arg.Is<int>(t => t == numberRowsPerPage));

            Assert.Equal(mockPokemon.Count, result.Count);
        }

        [Fact, Description("Test the get pokemons paginated, return null since it is out of range")]
        public void GetPokemonPaginated_PageNumberAndRowsPerPage_ReturnsNull()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var mockPokemon = new List<Pokemon>
            {
                new Pokemon
                {
                    Name = "Pikachu",
                    Number = 25
                },
                new Pokemon
                {
                    Name = "Charmander",
                    Number = 4
                }
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.CountLinesInFile()
                .Returns(mockPokemon.Count);
            pokemonRepository.GetAllPaginated(Arg.Any<int>(), Arg.Any<int>())
                .Returns(mockPokemon);
            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            var outOfRangePageNumber = 3;
            var numberRowsPerPage = 2;

            // Act
            var result = pokemonService.GetPokemonPaginated(outOfRangePageNumber, numberRowsPerPage);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddNew Pokemon Tests
        [Fact, Description("Test the create new pokemon, returns an Ok message")]
        public void AddPokemon_Pokemon_ReturnsOkMessage()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var pokemon = new Pokemon
            {
                Name = "Pikachu",
                Number = 25
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.AddNew(Arg.Any<Pokemon>())
                .Returns(pokemon);
            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.AddPokemon(pokemon);

            // Assert
            pokemonRepository
                .Received(1)
                .AddNew(Arg.Is<Pokemon>(
                    t => t.Name == pokemon.Name
                    && t.Number == pokemon.Number
                ));

            Assert.Equal(ResultMessage.Ok, result);
        }

        [Fact, Description("Test the create new pokemon, returns the pokemon created")]
        public void AddPokemon_Pokemon_ReturnsAlreadyExists()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var pokemon = new Pokemon
            {
                Name = "Pikachu",
                Number = 25
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.GetByName(Arg.Any<string>())
               .Returns(pokemon);
            pokemonRepository.AddNew(Arg.Any<Pokemon>())
                .Returns(pokemon);
            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.AddPokemon(pokemon);

            // Assert            
            Assert.Equal(ResultMessage.Exists, result);
        }

        [Fact, Description("Test the create new pokemon, returns error")]
        public void AddPokemon_Pokemon_Error()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var pokemon = new Pokemon
            {
                Name = "Pikachu",
                Number = 25
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.AddPokemon(pokemon);

            // Assert            
            Assert.Equal(ResultMessage.Error, result);
        }
        #endregion

        #region Modify Pokemon Tests
        [Fact, Description("Test the update pokemon without changing the Name, returns an Updated message")]
        public void ModifyPokemon_PokemonSameName_ReturnsUpdatedMessage()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var pokemon = new Pokemon
            {
                Name = "Pikachu",
                Number = 25
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.Modify(Arg.Any<string>(), Arg.Any<Pokemon>())
                .Returns(ResultMessage.Updated);
            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var originalName = "Pikachu";
            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.ModifyPokemon(originalName, pokemon);

            // Assert
            pokemonRepository
                .Received(1)
                .Modify(Arg.Is<string>(
                    t => t == originalName
                )
                , Arg.Is<Pokemon>(
                    t => t.Name == pokemon.Name
                    && t.Number == pokemon.Number
                ));

            Assert.Equal(ResultMessage.Updated, result);
        }

        [Fact, Description("Test the update pokemon with a new Name, returns an Updated message")]
        public void ModifyPokemon_PokemonNewName_ReturnsUpdatedMessage()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var pokemon = new Pokemon
            {
                Name = "Pikachu",
                Number = 25
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.Modify(Arg.Any<string>(), Arg.Any<Pokemon>())
                .Returns(ResultMessage.Updated);
            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var originalName = "Test";
            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.ModifyPokemon(originalName, pokemon);

            // Assert
            pokemonRepository
                .Received(1)
                .Modify(Arg.Is<string>(
                    t => t == originalName
                )
                , Arg.Is<Pokemon>(
                    t => t.Name == pokemon.Name
                    && t.Number == pokemon.Number
                ));

            Assert.Equal(ResultMessage.Updated, result);
        }

        [Fact, Description("Test the update pokemon, returns an Already Exists message")]
        public void ModifyPokemon_Pokemon_ReturnsExistsMessage()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };

            var pokemon = new Pokemon
            {
                Name = "Pikachu",
                Number = 25
            };

            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.GetByName(Arg.Any<string>())
               .Returns(pokemon);
            pokemonRepository.Modify(Arg.Any<string>(), Arg.Any<Pokemon>())
                .Returns(ResultMessage.Updated);
            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var originalName = "Test";
            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.ModifyPokemon(originalName, pokemon);

            // Assert
            pokemonRepository
                .Received(0);

            Assert.Equal(ResultMessage.Exists, result);
        }
        #endregion

        #region Delete Pokemon Tests
        [Fact, Description("Test the delete pokemon, returns an deleted message")]
        public void DeletePokemon_PokemonSameName_ReturnsUpdatedMessage()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                ApplicationConfiguration = new ApplicationConfiguration { CsvPath = "TestPath" }
            };
         
            var logger = Substitute.For<ILogger<PokemonService>>();
            var options = Substitute.For<IOptions<AppSettings>>();
            var pokemonRepository = Substitute.For<IPokemonRepository>();
            var fileSystem = Substitute.For<IFileSystem>();

            pokemonRepository.Delete(Arg.Any<string>())
                .Returns(ResultMessage.Deleted);
            options.Value
                .Returns(appSettings);
            fileSystem.File.Exists(Arg.Any<string>())
                .Returns(true);

            var pokemonName = "Pikachu";
            var pokemonService = new PokemonService(options, pokemonRepository, logger, fileSystem);

            // Act
            var result = pokemonService.DeletePokemon(pokemonName);

            // Assert
            pokemonRepository
                .Received(1)
                .Delete(Arg.Is<string>(
                    t => t == pokemonName
                ));

            Assert.Equal(ResultMessage.Deleted, result);
        }
        #endregion
    }
}
