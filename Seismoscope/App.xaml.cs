using System.Windows;
using Seismoscope.Utils;
using Seismoscope.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Seismoscope.Model.Interfaces;
using Seismoscope.Model.DAL;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.Utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Seismoscope.View;

namespace Seismoscope
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            // Note à moi-même, mieux séparer en fonctions ici. 
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
            IConfiguration configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<ConnectUserViewModel>();
            services.AddSingleton<CarteViewModel>(); 
            services.AddSingleton<StationViewModel>();
            services.AddSingleton<CapteurViewModel>();

            services.AddSingleton<HistoriqueEvenementsViewModel>();

            services.AddSingleton<DonneesCapteurViewModel>();
            services.AddSingleton<IStationService, StationService>();
            services.AddSingleton<ICapteurService, CapteurService>();
            services.AddSingleton<IUserSessionService, UserSessionService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IEvenementService, EvenementService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IStationDAL, StationDAL>();
            services.AddSingleton<ICapteurDAL, CapteurDAL>();
            services.AddSingleton<IEvenementDAL, EvenementDAL>();
            services.AddSingleton<IEvenementService, EvenementService>();


            services.AddSingleton<IUserDAL, UserDAL>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserSessionService, Service>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<Func<Type, BaseViewModel>>(serviceProvider =>
            {
                BaseViewModel ViewModelFactory(Type viewModelType)
                {
                    return (BaseViewModel)serviceProvider.GetRequiredService(viewModelType);
                }
                return ViewModelFactory;
            });

            services.AddDbContext<ApplicationDbContext>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (dbContext.Database.EnsureCreated())
                {
                    dbContext.SeedData();
                }
                //var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //dbContext.Database.Migrate();
                //dbContext.SeedData();

            }

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
