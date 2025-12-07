using ExpensesAndStuff.Interfaces;
using ExpensesAndStuff.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;

namespace ExpensesAndStuff
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // FORCE SWEDISH

            CultureInfo swedish = new CultureInfo("sv-SE");
            Thread.CurrentThread.CurrentCulture = swedish;
            Thread.CurrentThread.CurrentUICulture = swedish;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage("sv-SE"))
            );

            var services = new ServiceCollection();

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var DbPath = System.IO.Path.Join(path, "budgetplanner.db");

            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={DbPath}"));

            // Services
            services.AddTransient<ExpenseService>();
            services.AddTransient<IncomeService>();
            services.AddTransient<AbsenceService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            // Windows
            services.AddTransient<MainWindow>();

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
