using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using GuardaImagenNet6.Models;

namespace GuardaImagenNet6.Models.Contexto
{
    public partial class PruebaDBContext : DbContext
    {
        public PruebaDBContext()
        {
        }

        public PruebaDBContext(DbContextOptions<PruebaDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

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
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("userName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
