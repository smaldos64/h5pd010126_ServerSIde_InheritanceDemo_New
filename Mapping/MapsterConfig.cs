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
            // 1. FAG: Map DTO.Id til Entity.FagId og omvendt
            //config.NewConfig<Fag, FagDto>()
            //    .Map(dest => dest.Id, src => src.FagId);

            //config.NewConfig<FagDto, Fag>()
            //    .Map(dest => dest.FagId, src => src.Id);

            // 2. HOLD: Map DTO.Id til Entity.HoldId og omvendt
            //config.NewConfig<Hold, HoldDto>()
            //    .Map(dest => dest.Id, src => src.HoldId);

            //config.NewConfig<HoldDto, Hold>()
            //    .Map(dest => dest.HoldId, src => src.Id);

            // 3. STUDENT: Håndtering af fladtrykning (HoldNavn) og ignorering af samlinger
            //config.NewConfig<Student, StudentDto>()
            //    .Map(dest => dest.HoldNavn, src => src.Hold != null ? src.Hold.HoldNavn : null)
            //    .Map(dest => dest.FagTitler, src => src.Fag.Select(f => f.FagTitel));

            //config.NewConfig<StudentUpdateDto, Student>()
            //    .Ignore(dest => dest.Fag)  // Håndteres manuelt af SynchronizeManyToMany
            //    .Ignore(dest => dest.Hold); // Vi vil typisk kun opdatere HoldId (int)

            // 4. ANSAT: Fladtrykning af AfdelingNavn
            //config.NewConfig<Ansat, AnsatDto>()
            //    .Map(dest => dest.AfdelingNavn, src => src.Afdeling != null ? src.Afdeling.AfdelingNavn : null);
            //config.NewConfig<AnsatDto, Ansat>()
            //    .Map(dest => dest.AfdelingId, src => src.Id);

            //config.NewConfig<Ansat, AnsatDto>()
            //    .Map(dest => dest.Id, src => src.AfdelingId);

            // 5. AFDELING
            //config.NewConfig<Afdeling, AfdelingDto>()
            //    .Map(dest => dest.Id, src => src.AfdelingId);

            //config.NewConfig<AfdelingDto, Afdeling>()
            //    .Map(dest => dest.AfdelingId, src => src.Id);

            // --- POLYMORFISK MAPPING (Til POST/PUT) ---
            // Dette sikrer, at hvis du sender en liste af PersonDto, 
            // så ved Mapster hvilken sub-type der skal oprettes.
            config.NewConfig<PersonDto, Person>()
                .Include<StudentDto, Student>()
                .Include<AnsatDto, Ansat>();



            // Gør konfigurationen global og "klar til brug"
            //TypeAdapterConfig.GlobalSettings.Scan(System.Reflection.Assembly.GetExecutingAssembly());


            // Gamle erklæringer herunder !!!
            // --- STUDENT MAPPING ---
            //config.NewConfig<Student, StudentDto>()
            //.Inherits<Person, PersonDto>()
            //.Map(dest => dest.HoldNavn, src => src.Hold.HoldNavn)
            //.Map(dest => dest.FagTitler, src => src.Fag.Select(f => f.FagTitel));

            // --- ANSAT MAPPING ---
            //config.NewConfig<Ansat, AnsatDto>()
            //    .Inherits<Person, PersonDto>()
            //    .Map(dest => dest.AfdelingNavn, src => src.Afdeling != null ? src.Afdeling.AfdelingNavn : null);

            // --- POLYMORFISK MAPPING (Til POST/PUT) ---
            // Dette sikrer, at hvis du sender en liste af PersonDto, 
            // så ved Mapster hvilken sub-type der skal oprettes.
            //config.NewConfig<PersonDto, Person>()
            //    .Include<StudentDto, Student>()
            //    .Include<AnsatDto, Ansat>();

            // Mange-til-mange: Undgå at Mapster prøver at oprette nye Fag-objekter
            // når vi opdaterer en Student. Vi mapper kun ID'er.
            //config.NewConfig<StudentUpdateDto, Student>()
            //    .Ignore(dest => dest.Fag); // Håndter fag manuelt i controlleren for fuld kontrol
        }

        public static void RegisterGlobal()
        {
            TypeAdapterConfig global = TypeAdapterConfig.GlobalSettings;
            Register(global);

            global.Scan(Assembly.GetExecutingAssembly());
        }
    }
}

