using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    public class TeacherCreateDto
    {
        public List<int> FagIds { get; set; } = new();

        public List<int> AfdelingsIds { get; set; } = new();
    }

    public class TeacherUpdateDto : TeacherCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class TeacherWithFagDto : PersonDto
    {
        public List<FagUpdateDto> Fag { get; set;} = new();
    }

    public class TeacherWithAfdelingDto : PersonDto
    {
        public List<AfdelingUpdateDto> Afdelinger { get; set; } = new();
    }

    public class TeacherDto : PersonDto
    {
        public List<FagUpdateDto> Fag { get; set; } = new();
        public List<AfdelingUpdateDto> Afdelinger { get; set; } = new();
    }
}


