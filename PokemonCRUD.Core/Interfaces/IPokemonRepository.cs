using PokemonCRUD.Core.Models;
using System.Collections.Generic;

namespace PokemonCRUD.Core.Interfaces
{
    public interface IPokemonRepository
    {
        Pokemon GetByName(string name);
        List<Pokemon> GetAllPaginated(int startingRow, int numberRowsPerPage);
        string AddNew(Pokemon pokemon);
        long CountLinesInFile();
    }
}
