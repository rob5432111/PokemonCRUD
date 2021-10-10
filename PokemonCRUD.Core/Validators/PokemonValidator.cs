using FluentValidation;
using PokemonCRUD.Core.Models;

namespace PokemonCRUD.Core.Validators
{
    public class PokemonValidator : AbstractValidator<Pokemon>
    {
        public PokemonValidator()
        {
            RuleFor(x => x.Number).Must(x => x >= 0).WithMessage("The Number can't be less than zero");
            RuleFor(x => x.Name).NotEmpty().WithMessage("The Name of the Pokemon can't be empty");
            RuleFor(x => x.Type1).NotEmpty().WithMessage("The Type 1 of the Pokemon can't be empty");
            RuleFor(x => x.Total).Must(x => x >= 0).WithMessage("The total can't be less than zero");
            RuleFor(x => x.HP).Must(x => x >= 0).WithMessage("The HP can't be less than zero");
            RuleFor(x => x.Attack).Must(x => x >= 0).WithMessage("The Attack can't be less than zero");
            RuleFor(x => x.Defense).Must(x => x >= 0).WithMessage("The Defense can't be less than zero");
            RuleFor(x => x.SpAttack).Must(x => x >= 0).WithMessage("The SpAttack can't be less than zero");
            RuleFor(x => x.SpDefense).Must(x => x >= 0).WithMessage("The SpDefense can't be less than zero");
            RuleFor(x => x.Speed).Must(x => x >= 0).WithMessage("The Speed can't be less than zero");
            RuleFor(x => x.Generation).Must(x => x >= 0).WithMessage("The Generation can't be less than zero");
            RuleFor(x => x.Legendary).NotNull();
        }
    }
}
