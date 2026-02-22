using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    public class TeacherCreateDto : PersonCreateDto
    {
        public List<int> FagIds { get; set; } = new();

        public List<int> AfdelingsIds { get; set; } = new();
    }

    public class TeacherUpdateDto : TeacherCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class TeacherWithFagDto : TeacherUpdateDto
    {
        public List<FagUpdateDto> Fag { get; set;} = new();
    }

    public class TeacherWithAfdelingDto : TeacherUpdateDto
    {
        public List<AfdelingUpdateDto> Afdelinger { get; set; } = new();
    }

    public class TeacherDto : TeacherUpdateDto
    {
        public List<FagUpdateDto> Fag { get; set; } = new();
        public List<AfdelingUpdateDto> Afdelinger { get; set; } = new();
    }
}


