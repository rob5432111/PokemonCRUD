using PokemonCRUD.Core.Models;
using System.Collections.Generic;

namespace PokemonCRUD.Core.Interfaces
{
    public interface IPokemonService
    {
        Pokemon GetPokemonByName(string name);

        List<Pokemon> GetPokemonPaginated(int startingRow, int numberRowsPerPage);
        Pokemon AddPokemon(Pokemon newPokemon);
    }
}
