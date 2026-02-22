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

    public class EUXStudentWithHoldDto : EUXStudentUpdateDto
    {
        public HoldUpdateDto? Hold { get; set; }
    }

    public class EUXStudentWithFagDto : EUXStudentUpdateDto
    {
        public List<FagUpdateDto> Fag { get; set; }
    }

    public class EUXStudentDto : EUXStudentUpdateDto
    {
        public HoldUpdateDto? Hold { get; set; }
        public List<FagUpdateDto> Fag { get; set; }
    }
}
