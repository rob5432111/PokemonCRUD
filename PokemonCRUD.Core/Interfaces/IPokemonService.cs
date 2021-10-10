using PokemonCRUD.Core.Models;
using System.Collections.Generic;

namespace PokemonCRUD.Core.Interfaces
{
    public interface IPokemonService
    {
        Pokemon GetPokemonByName(string name);
        List<Pokemon> GetPokemonPaginated(int startingRow, int numberRowsPerPage);
        string AddPokemon(Pokemon newPokemon);
        string ModifyPokemon(string originalName, Pokemon newPokemon);
        string DeletePokemon(string originalName);
    }
}
