using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using GuardaImagenNet6.Models;

namespace GuardaImagenNet6.Models.Contexto
{
    public partial class PruebasDBContext : DbContext
    {
        public PruebasDBContext()
        {
        }

        public PruebasDBContext(DbContextOptions<PruebasDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.UserName, "UQ__Usuarios__66DCF95C24125447")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Estatus)
                    .IsRequired()
                    .HasColumnName("estatus")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.FechaAlta)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaAlta")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FechaModifico)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaModifico")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FotoBd).HasColumnName("fotoBD");

                entity.Property(e => e.FotoPath)
                    .HasMaxLength(3000)
                    .IsUnicode(false)
                    .HasColumnName("fotoPath");

                entity.Property(e => e.IdUsuarioAlta).HasColumnName("idUsuarioAlta");

                entity.Property(e => e.IdUsuarioModifico).HasColumnName("idUsuarioModifico");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.UserName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("userName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
