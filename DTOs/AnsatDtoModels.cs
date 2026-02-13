using FluentValidation;
using InheritanceDemo.DTOInterfaces;
using InheritanceDemo.Models;

namespace InheritanceDemo.DTOs
{
    // POST: Oprettelse
    public class AnsatCreateDto : PersonCreateDto
    {
        public decimal MaanedsLoen { get; set; }
        public int AfdelingId { get; set; }
    }

    // Bruges ved HTTP PUT
    public class AnsatUpdateDto : AnsatCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    // GET: Læsning
    public class AnsatDto : PersonDto
    {
        public decimal MaanedsLoen { get; set; }
        //public string AfdelingNavn { get; set; } = String.Empty;
        public AfdelingUpdateDto Afdeling { get; set; } = new AfdelingUpdateDto();
        // Mapster flader denne ud fra Afdeling.Navn
    }

    public class AnsatCreateDtoValidator : AbstractValidator<AnsatCreateDto>
    {
        public AnsatCreateDtoValidator()
        {
            RuleFor(a => a.Email).
            NotEmpty().WithMessage("Email er påkrævet.").
            EmailAddress().WithMessage("Den indtastede E-mail adresse er ikke gyldig !!!");
        }
    }
}
