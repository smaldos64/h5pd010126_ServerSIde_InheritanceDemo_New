using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    public class EUDStudentCreateDto : StudentCreateDto
    {
        public string Laereplads { get; set; }
    }

    public class EUDStudentUpdateDto : EUDStudentCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class EUDStudentWithHoldDto : StudentWithHoldDto
    {
        public HoldUpdateDto? Hold { get; set; }
    }

    public class EUDStudentWithFagDto : StudentWithFagDto
    {
        public List<FagUpdateDto> Fag { get; set; }
    }

    public class EUDStudentDto : StudentDto
    {
    }
}
