using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Samples.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Epam.GraphQL.Samples.Data
{
    public partial class GraphQLDbContext : DbContext
    {
        public DbSet<Language> Languages { get; set; }

        public DbSet<Continent> Continents { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<CountryLanguage> CountryLanguages { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<User> Users { get; set; }

        public GraphQLDbContext()
            : base(new DbContextOptionsBuilder<GraphQLDbContext>().UseSqlite("DataSource=demo.db").Options)
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
            modelBuilder.Entity<Language>(
                entity => entity.HasKey(l => l.Code));

            modelBuilder.Entity<Continent>(
                entity => entity.HasKey(c => c.Code));

            modelBuilder.Entity<Currency>(
                entity => entity.HasKey(c => c.AlphabeticCode));

            modelBuilder.Entity<Country>(
                entity =>
                {
                    entity.HasKey(c => c.Code);

                    entity
                        .HasOne(c => c.Currency)
                        .WithMany(c => c.Countries)
                        .HasForeignKey(c => c.CurrencyAlphabeticCode);

                    entity
                        .HasOne(c => c.Continent)
                        .WithMany(c => c.Countries)
                        .HasForeignKey(c => c.ContinentCode)
                        .IsRequired();
                });

            modelBuilder.Entity<CountryLanguage>(
                entity =>
                {
                    entity.HasKey(l => new
                    {
                        l.CountryCode,
                        l.LanguageCode,
                    });

                    entity.HasOne(l => l.Country)
                        .WithMany(c => c.Languages)
                        .HasForeignKey(l => l.CountryCode);

                    entity
                        .HasOne(l => l.Language)
                        .WithMany(l => l.Countries)
                        .HasForeignKey(l => l.LanguageCode);
                });

            modelBuilder.Entity<City>(
                entity =>
                {
                    entity.HasKey(c => c.Id);

                    entity
                        .HasOne(c => c.Country)
                        .WithMany(c => c.Cities)
                        .HasForeignKey(c => c.CountryCode);
                });

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

            modelBuilder.Entity<Language>()
                .HasData(GetLanguages());

            modelBuilder.Entity<Continent>()
                .HasData(GetContinents());

            modelBuilder.Entity<Currency>()
                .HasData(GetCurrencies());

            modelBuilder.Entity<Country>()
                .HasData(GetCountries());

            modelBuilder.Entity<CountryLanguage>()
                .HasData(GetCountryLanguages());

            modelBuilder.Entity<City>()
                .HasData(GetCities());
        }
    }
}
