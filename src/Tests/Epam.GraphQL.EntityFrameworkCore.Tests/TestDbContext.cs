// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Tests.TestData;
using Microsoft.EntityFrameworkCore;

namespace Epam.GraphQL.EntityFrameworkCore.Tests
{
    public class TestDbContext : DbContext
    {
        public TestDbContext()
        {
        }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> People { get; set; }

        public DbSet<Unit> Units { get; set; }

        public DbSet<PersonSettings> PeopleSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            modelBuilder.Entity<Unit>(
#pragma warning restore CA1062 // Validate arguments of public methods
                entity =>
                {
                    entity
                        .HasOne(u => u.Head)
                        .WithOne(p => p.IsInChargeOf)
                        .HasForeignKey<Unit>(p => p.HeadId);
                });

            modelBuilder.Entity<Person>(
                entity =>
                {
                    entity
                        .HasOne(p => p.Unit)
                        .WithMany(u => u.Employees)
                        .HasForeignKey(p => p.UnitId);
                });
            modelBuilder.Entity<PersonSettings>(
                entity =>
                {
                    entity
                        .HasKey(p => p.Id);
                    entity.Property(p => p.Id)
                        .ValueGeneratedNever()
                        .IsRequired();
                    entity
                        .HasOne(p => p.Person)
                        .WithOne(u => u.Settings)
                        .HasForeignKey<PersonSettings>(p => p.Id);
                });
        }
    }
}
