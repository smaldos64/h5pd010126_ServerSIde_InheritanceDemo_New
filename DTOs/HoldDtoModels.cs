using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    public class HoldCreateDto
    {
        public string? HoldNavn { get; set; }
    }

    public class HoldUpdateDto : HoldCreateDto
    {
        public int HoldId { get; set; }
    }

    public class HoldDto : HoldUpdateDto
    {
        public List<StudentWithFagDto>? Studerende { get; set; }
    }
}
