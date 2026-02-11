using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    // 1. Base Create DTO (uden ID)
    public class PersonCreateDto
    {
        public string Navn { get; set; } = string.Empty;
        public int Alder { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public class PersonWithIDto : PersonCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class PersonUpdateDto : PersonWithIDto
    {
    }

    public class PersonDto : PersonWithIDto
    {
    }
}
