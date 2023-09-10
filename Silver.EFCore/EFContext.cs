using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Peak.Lib.EFCore.EFAssembly;
using System.Reflection;

namespace Peak.Lib.EFCore
{
    public partial class EFContext : DbContext
    {
        private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => {  });
        private static List<EFModelAssembly> Efassembly = new List<EFModelAssembly>();
        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.MappingEntityTypes(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void MappingEntityTypes(ModelBuilder modelBuilder)
        {
            if (Efassembly.Count <= 0)
            {
                return;
            }
            foreach (var item in Efassembly)
            {
                var listAssembly = Assembly.Load(new AssemblyName(item.NameSpaces)).GetTypes().Where(t => t.IsClass && !t.IsGenericType && !t.IsAbstract).ToList();
                if (listAssembly != null && listAssembly.Any())
                {
                    listAssembly.ForEach(assembly =>
                    {
                        try
                        {
                            if (modelBuilder.Model.FindEntityType(assembly) == null)
                            {
                                if (assembly.FullName.Contains(item.FullName))
                                {
                                    modelBuilder.Model.AddEntityType(assembly);
                                }
                            }
                        }
                        catch { }
                    });
                }
            }
        }

    }

}
