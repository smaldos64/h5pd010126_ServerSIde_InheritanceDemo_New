using InheritanceDemo.DTOInterfaces;
using InheritanceDemo.Models;

namespace InheritanceDemo.DTOs
{
    public class FagCreateDto
    {
        public string? FagTitel { get; set; }
    }

    public class FagUpdateDto : FagCreateDto
    {
        public int FagId { get; set; }
    }

    public class FagDto : FagUpdateDto
    {
        public List<StudentDtoWithHold>? Studerende { get; set; }
    }
}
