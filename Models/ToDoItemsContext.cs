using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.EFCore.CosmosDb.Sample.Models
{
    public class ToDoItemsContext : DbContext
    {
        private readonly DbContextOptions<ToDoItemsContext> _options;
        private readonly IConfiguration _configuration;
        private IConfigurationSection _cosmosSettings;

        public ToDoItemsContext(DbContextOptions<ToDoItemsContext> options, IConfiguration configuration)
        {
            _options = options;
            _configuration = configuration;
        }
        
        public DbSet<ToDoItem> ToDoItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _cosmosSettings = _configuration.GetSection("CosmosDb");
            
            optionsBuilder.UseCosmos(
                _cosmosSettings["AccountEndpoint"],
                _cosmosSettings["AccountKeys"],
                _cosmosSettings["Database"])
                .EnableSensitiveDataLogging();

            Console.Write($"State of {nameof(optionsBuilder.IsConfigured)} as: {optionsBuilder.IsConfigured}");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer(_cosmosSettings["Collection"]);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}