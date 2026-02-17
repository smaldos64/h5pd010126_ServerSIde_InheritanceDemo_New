using FluentValidation;
using InheritanceDemo.DTOInterfaces;
using InheritanceDemo.Models;

namespace InheritanceDemo.DTOs
{
    // Bruges ved HTTP POST
    public class StudentCreateDto : PersonCreateDto
    {
        public int HoldId { get; set; }
        // En simpel liste af ID'er fra klienten
        public List<int> FagIds { get; set; } = new();
    }

    // Bruges ved HTTP PUT
    public class StudentUpdateDto : StudentCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class StudentWithHoldDto : PersonDto
    {
        public HoldUpdateDto? Hold { get; set; }
    }

    public class StudentWithFagDto : PersonDto
    {
        public List<FagUpdateDto> Fag { get; set; }
    }

    public class StudentDto : PersonDto
    {
        public HoldUpdateDto? Hold { get; set; }
        public List<FagUpdateDto>? Fag { get; set; }
    }

    public class StudentCreateDtoValidator : AbstractValidator<StudentCreateDto>
    {
        public StudentCreateDtoValidator()
        {
            // Validerer på relationen/listen
            RuleFor(s => s.FagIds)
                .NotEmpty().WithMessage("En studerende skal være tilmeldt mindst ét fag.")
                //.Must(fagListe => fagListe.Count <= 3).WithMessage("En studerende kan højst have 3 fag.")
                .Must(FagIds => FagIds.Count <= 3).WithMessage("En studerende kan højst have 3 fag.");
        }
    }
}
