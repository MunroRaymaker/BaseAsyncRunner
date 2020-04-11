using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AutoMapper;
using CommandLine;
using Ninject;
using Ninject.Modules;

namespace BaseAsyncRunner
{
    /// <summary>
    /// Bind all dependencies here.
    /// Usage:
    /// var container = new Kernel(new ConfigureContainer())
    /// or use
    /// var container = new StandardKernel();
    /// new Kernel().Load(Assembly.GetExecutingAssembly());
    /// </summary>
    public class ConfigureContainer : NinjectModule
    {
        public override void Load()
        {
            Bind<ILog>().To<Log>().InTransientScope();
            Bind<IConfig>().To<Config>().InSingletonScope();
            Bind<IMapper>().ToMethod(AutoMapper).InSingletonScope();
        }

        private MapperConfiguration CreateConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Add all profiles in current assembly
                cfg.AddProfiles(GetType().Assembly);
                //  mc.AddProfile(new MappingProfile());
            });

            return config;
        }

        private IMapper AutoMapper(Ninject.Activation.IContext context)
        {
            Mapper.Initialize(config =>
            {
                config.ConstructServicesUsing(type => context.Kernel.Get(type));

                //config.CreateMap<Customer, CustomerDto>();
                // .... other mappings, Profiles, etc.              
                //config.AddProfile(new MappingProfile());
                config.AddProfiles(GetType().Assembly);
            });

            Mapper.AssertConfigurationIsValid(); // optional
            return Mapper.Instance;
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Customer, Domain.CustomerDto>();
            CreateMap<Domain.CustomerDto, Domain.Customer>();
        }
    }
}
