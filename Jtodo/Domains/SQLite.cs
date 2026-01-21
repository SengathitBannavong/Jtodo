using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Jtodo.Domains
{
    public class SQLite : DbContext
    {
        private static readonly string _database_path;

        static SQLite()
        {
            // E:\Project I\Jtodo\Jtodo\Data\TodoListDB.db
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            
            // Out of bin\Debug\net10.0-windows 
            var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", ".."));
            var dataFolder = Path.Combine(projectRoot, "Data");
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
                Console.WriteLine($"[INFO] Created Data folder: {dataFolder}");
            }
            
            _database_path = Path.Combine(dataFolder, "TodoListDB.db");
            Console.WriteLine($"[INFO] Database path: {_database_path}");
            Console.WriteLine($"[INFO] File exists: {File.Exists(_database_path)}");
            
            if (File.Exists(_database_path))
            {
                var fileInfo = new FileInfo(_database_path);
                Console.WriteLine($"[INFO] File size: {fileInfo.Length} bytes");
                Console.WriteLine($"[INFO] Last modified: {fileInfo.LastWriteTime}");
            }
        }

        public DbSet<TodoList> TodoLists { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<TodoListItem> TodoListItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_database_path}");
            
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Type entity
            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("type");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Text).HasColumnName("text");
                entity.Property(e => e.Color).HasColumnName("color");
            });

            // Configure TodoList entity
            modelBuilder.Entity<TodoList>(entity =>
            {
                entity.ToTable("todo_list");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .IsRequired()
                    .HasConversion(
                        v => $"{v:yyyy-MM-dd}",
                        v => DateTime.ParseExact(v, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
                    );
                
                // Ignore in-memory collection (will use junction table)
                entity.Ignore(e => e.Todo_Items);
            });

            // Configure TodoItem entity
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.ToTable("todo_item");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasConversion(
                        v => $"{v:yyyy-MM-dd}",
                        v => DateTime.ParseExact(v, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
                    );
                entity.Property(e => e.DueDate)
                    .HasColumnName("due_date")
                    .HasConversion(
                        v => $"{v:yyyy-MM-dd}",
                        v => DateTime.ParseExact(v, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
                    );
                
                // Priority: enum -> TEXT conversion (PascalCase -> snake_case)
                entity.Property(e => e.Priority)
                    .HasColumnName("priority")
                    .HasConversion(
                        v => ConvertToSnakeCase(v.ToString()),
                        v => Enum.Parse<Values.Priority>(ConvertToPascalCase(v), true)
                    )
                    .HasDefaultValue(Values.Priority.Low);
                
                // Status: enum -> TEXT conversion (PascalCase -> snake_case)
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion(
                        v => ConvertToSnakeCase(v.ToString()),
                        v => Enum.Parse<Values.Status>(ConvertToPascalCase(v), true)
                    )
                    .HasDefaultValue(Values.Status.Pending);
                
                entity.Property(e => e.TypeId).HasColumnName("type_id").IsRequired();

                // Foreign Key to Type
                entity.HasOne<Type>()
                    .WithMany()
                    .HasForeignKey(e => e.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure TodoListItem junction table (Many-to-Many)
            modelBuilder.Entity<TodoListItem>(entity =>
            {
                entity.ToTable("todo_list_item");
                
                // Composite Primary Key
                entity.HasKey(e => new { e.TodoListId, e.TodoItemId });
                
                entity.Property(e => e.TodoListId).HasColumnName("todo_list_id");
                entity.Property(e => e.TodoItemId).HasColumnName("todo_item_id");

                // Foreign Key to TodoList
                entity.HasOne(e => e.TodoList)
                    .WithMany()
                    .HasForeignKey(e => e.TodoListId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key to TodoItem
                entity.HasOne(e => e.TodoItem)
                    .WithMany()
                    .HasForeignKey(e => e.TodoItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // Helper method: Convert PascalCase to snake_case
        private static string ConvertToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var result = new System.Text.StringBuilder();
            result.Append(char.ToLower(text[0]));

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    result.Append('_');
                    result.Append(char.ToLower(text[i]));
                }
                else
                {
                    result.Append(text[i]);
                }
            }

            return result.ToString();
        }

        // Helper method: Convert snake_case to PascalCase
        private static string ConvertToPascalCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var parts = text.Split('_');
            var result = new System.Text.StringBuilder();

            foreach (var part in parts)
            {
                if (part.Length > 0)
                {
                    result.Append(char.ToUpper(part[0]));
                    if (part.Length > 1)
                    {
                        result.Append(part.Substring(1));
                    }
                }
            }

            return result.ToString();
        }
    }
}
