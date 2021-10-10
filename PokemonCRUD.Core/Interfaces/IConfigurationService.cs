using System.Threading.Tasks;

namespace PokemonCRUD.Core.Interfaces
{
    public interface IConfigurationService
    {
        string ConfigureCsvPath(string path);
    }
}
