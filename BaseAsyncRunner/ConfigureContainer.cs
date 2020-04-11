using AutoMapper;
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
}
