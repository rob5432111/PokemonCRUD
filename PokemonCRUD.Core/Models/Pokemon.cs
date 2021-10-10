namespace PokemonCRUD.Core.Models
{
    public class Pokemon
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public int Total { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpAttack { get; set; }
        public int SpDefense { get; set; }
        public int Speed { get; set; }
        public int Generation { get; set; }
        public bool Legendary { get; set; }
    }  
}
