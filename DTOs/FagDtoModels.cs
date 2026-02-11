using InheritanceDemo.DTOInterfaces;
using InheritanceDemo.Models;

namespace InheritanceDemo.DTOs
{
    public class FagCreateDto
    {
        public string? FagTitel { get; set; }
    }

    public class FagUpdateDto : FagCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class FagDto : FagUpdateDto
    {
        public List<StudentDto>? Studerende { get; set; }
    }
}
