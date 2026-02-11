using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    public class HoldCreateDto
    {
        public string? HoldNavn { get; set; }
    }

    public class HoldUpdateDto : HoldCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class HoldDto : HoldUpdateDto
    {
        public List<StudentDto>? Studerende { get; set; }
    }
    
}
