using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SMS.Model
{
    public partial class PlivoCodingAssignmentContext : DbContext
    {
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<PhoneNumber> PhoneNumber { get; set; }
        public static String ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AuthId)
                    .HasColumnName("auth_id")
                    .HasMaxLength(40);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<PhoneNumber>(entity =>
            {
                entity.ToTable("phone_number");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.Number)
                    .HasColumnName("number")
                    .HasMaxLength(40);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.PhoneNumber)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_phone_number_account");
            });
        }
    }
}