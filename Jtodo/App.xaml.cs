using Jtodo.Interfaces;
using Jtodo.Repositories;
using Jtodo.Services;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace Jtodo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        private static IAppUnit? _unitOfWork;
        private static TodoListService? _todoListService;
        private static INavigationService? _navigationService;

        public static IAppUnit UnitOfWork
        {
            get
            {
                _unitOfWork ??= new SQLiteUnitOfWork();
                return _unitOfWork;
            }
        }

        public static TodoListService TodoListService
        {
            get
            {
                _todoListService ??= new TodoListService(UnitOfWork);
                return _todoListService;
            }
        }

        public static INavigationService NavigationService
        {
            get
            {
                _navigationService ??= new AppNavigationService();
                return _navigationService;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            AllocConsole();
            Console.WriteLine("=== Jtodo Application Started ===");
            Console.WriteLine($"Startup Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine();
#endif
            Console.WriteLine("=== Application Startup Complete ===");
            Console.WriteLine();
        }

        protected override void OnExit(ExitEventArgs e)
        {
#if DEBUG
            Console.WriteLine();
            Console.WriteLine("=== Application Exiting ===");
            Console.WriteLine($"Exit Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
#endif

            _unitOfWork?.Dispose();
            Console.WriteLine("[YES] Unit of Work disposed");

#if DEBUG
            Console.WriteLine("Press any key to close console...");
            Console.ReadKey();
            FreeConsole();
#endif

            base.OnExit(e);
        }
    }
}
