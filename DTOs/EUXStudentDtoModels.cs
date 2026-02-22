using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    public class EUXStudentCreateDto : StudentCreateDto
    {
        public int Uddannelseslaengde { get; set; }
    }

    public class EUXStudentUpdateDto : EUXStudentCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class EUXStudentWithHoldDto : StudentWithHoldDto
    {
        public HoldUpdateDto? Hold { get; set; }
    }

    public class EUXStudentWithFagDto : StudentWithFagDto
    {
        public List<FagUpdateDto> Fag { get; set; }
    }

    public class EUXStudentDto : StudentDto
    {
    }
}
