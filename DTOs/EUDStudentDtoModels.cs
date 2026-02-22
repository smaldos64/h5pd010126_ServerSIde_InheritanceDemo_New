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

    public class EUDStudentWithHoldDto : EUDStudentUpdateDto
    {
        public HoldUpdateDto? Hold { get; set; }
    }

    public class EUDStudentWithFagDto : EUDStudentUpdateDto
    {
        public List<FagUpdateDto> Fag { get; set; }
    }

    public class EUDStudentDto : EUDStudentUpdateDto
    {
        public HoldUpdateDto? Hold { get; set; }
        public List<FagUpdateDto>? Fag { get; set; }
    }
}
