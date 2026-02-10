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
            config.NewConfig<Student, StudentDto>()
                .Map(dest => dest.HoldNavn, src => src.Hold.HoldNavn)
                .Map(dest => dest.FagTitler, src => src.Fag.Select(f => f.FagTitel));

            //config.NewConfig<InventoryFile, InventoryFileCreateDto>()
            //    .Map(d => d.Url, s => s.RelativePath.MakeUrl());

            //config.NewConfig<InventoryFile, InventoryFileUpdateDto>()
            //    .Map(d => d.Url, s => s.RelativePath.MakeUrl());

            //config.NewConfig<InventoryFile, InventoryFileDto>()
            //    .Map(d => d.Url, s => s.RelativePath.MakeUrl());
        }

        public static void RegisterGlobal()
        {
            TypeAdapterConfig global = TypeAdapterConfig.GlobalSettings;
            Register(global);

            global.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
