using Jtodo.Repositories;
using Jtodo.Services;
using Jtodo.Services.Interfaces;
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

            try
            {
                Console.WriteLine("[INFO] Initializing Unit of Work...");
                var uow = UnitOfWork;
                Console.WriteLine("[YES] Unit of Work initialized successfully");
                Console.WriteLine();

                Console.WriteLine("[INFO] Reading TodoLists from database...");
                var todoLists = uow.TodoListRepository.Get_All_Todo_list();
                
                Console.WriteLine($"[YES] Successfully loaded {todoLists.Count} TodoList(s) from database");
                Console.WriteLine();

                if (todoLists.Count > 0)
                {
                    Console.WriteLine("[INFO] TodoList Details:");
                    foreach (var list in todoLists)
                    {
                        Console.WriteLine($"  - ID: {list.Id}");
                        Console.WriteLine($"    Title: {list.Title}");
                        Console.WriteLine($"    Description: {list.Description}");
                        Console.WriteLine($"    Items Count: {list.Todo_Items.Count}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("[INFO]  No TodoLists found in database");
                    Console.WriteLine("   Database might be empty or not initialized yet");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Error reading database:");
                Console.WriteLine($"   {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
            }

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
