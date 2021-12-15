using System;
using System.Collections.Generic;
using Epam.GraphQL.Samples.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Epam.GraphQL.Samples.Data
{
    public class GraphQLDbContext : DbContext
    {
        private SqliteConnection _connection = new SqliteConnection("DataSource=:memory:");
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }

        public GraphQLDbContext()
            : base(new DbContextOptionsBuilder<GraphQLDbContext>().UseSqlite("DataSource=:memory:").Options)
        {
            Database.OpenConnection();
            Database.EnsureCreated();
        }

        public override void Dispose()
        {
            Database.CloseConnection();
            base.Dispose();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var loggerFactory = LoggerFactory.Create(
                builder => builder
                    .ClearProviders()
                    .AddConsole()
                    .AddFilter((category, logLevel) => category == DbLoggerCategory.Database.Command.Name && logLevel == LogLevel.Information));

            optionsBuilder
                .UseLoggerFactory(loggerFactory)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(
                entity =>
                {
                    entity
                        .HasOne(u => u.Parent)
                        .WithMany(u => u.Children)
                        .HasForeignKey(u => u.ParentId);

                    entity
                        .HasOne(d => d.CreatedBy)
                        .WithMany()
                        .HasForeignKey(d => d.CreatedById);

                    entity
                        .HasOne(d => d.ModifiedBy)
                        .WithMany()
                        .HasForeignKey(d => d.ModifiedById);
                });

            modelBuilder.Entity<User>(
                entity =>
                {
                });



            modelBuilder.Entity<User>().HasData(
                    new User { Id = 1, FullName = "Ilya Kuznetsov" },
                    new User { Id = 2, FullName = "Yakov Zhmourov" }
                );

            modelBuilder.Entity<Department>().HasData(
                    new Department { Id = 1, Name = "Alpha", CreatedAt = new DateTime(2000, 3, 1, 10, 00, 30), CreatedById = 1, ModifiedAt = new DateTime(2000, 3, 1, 10, 00, 30), ModifiedById = 1 },
                    new Department { Id = 2, Name = "Beta", ParentId = 1, CreatedAt = new DateTime(2007, 5, 17, 10, 00, 30), CreatedById = 2, ModifiedAt = new DateTime(2008, 2, 28, 11, 03, 31), ModifiedById = 1 },
                    new Department { Id = 3, Name = "Gamma", ParentId = 1, CreatedAt = new DateTime(2003, 11, 27, 11, 37, 49), CreatedById = 1, ModifiedAt = new DateTime(2003, 12, 28, 13, 19, 01), ModifiedById = 2 }
                );
        }
    }
}
