using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using WebProject.Domain.Models;
using WebProject.Domain.Models.Books;


namespace WebProject.Persistence.Contexts
{
    public class BiblioDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<BooksOnShelves> BooksOnShelves => Set<BooksOnShelves>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<TakenBooks> TakenBooks => Set<TakenBooks>(); 
        public BiblioDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<User>();
            modelBuilder.Entity<UserRole>();
            modelBuilder.Entity<Book>();
            modelBuilder.Entity<Genre>();
            modelBuilder.Entity<BooksOnShelves>();
            modelBuilder.Entity<TakenBooks>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
