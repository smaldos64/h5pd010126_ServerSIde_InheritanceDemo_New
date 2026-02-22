using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Mapster;
using System.Reflection;

namespace InheritanceDemo.Mapping
{
    public static class MapsterConfig
    {
        public static void Register(TypeAdapterConfig config)
        {
            // --- POLYMORFISK MAPPING (Til POST/PUT) ---
            // Dette sikrer, at hvis du sender en liste af PersonDto, 
            // så ved Mapster hvilken sub-type der skal oprettes.
            config.NewConfig<PersonDto, Person>()
                //.Include<StudentDto, Student>()
                //.Include<EUXStudentDto, EUXStudent>()
                //.Include<EUDStudentDto, EUDStudent>()
                .Include<AnsatDto, Ansat>()
                .Include<TeacherDto, Teacher>();

            config.NewConfig<Student, StudentDto>()
               .Map(dest => dest.Hold.HoldId, src => src.HoldId)
               .Map(dest => dest.Hold.HoldNavn, src => src.Hold.HoldNavn);
        }

        public static void RegisterGlobal()
        {
            TypeAdapterConfig global = TypeAdapterConfig.GlobalSettings;
            Register(global);

            global.Scan(Assembly.GetExecutingAssembly());
        }
    }
}

