﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TamboliyaApi.Data;

#nullable disable

namespace TamboliyaApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("TamboliyaApi.Data.SideOfDodecahedron", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Color")
                        .HasColumnType("int");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SideOfDodecahedrons");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Color = 2,
                            Number = 1
                        },
                        new
                        {
                            Id = 2,
                            Color = 1,
                            Number = 2
                        },
                        new
                        {
                            Id = 3,
                            Color = 3,
                            Number = 3
                        },
                        new
                        {
                            Id = 4,
                            Color = 3,
                            Number = 4
                        },
                        new
                        {
                            Id = 5,
                            Color = 4,
                            Number = 5
                        },
                        new
                        {
                            Id = 6,
                            Color = 1,
                            Number = 6
                        },
                        new
                        {
                            Id = 7,
                            Color = 4,
                            Number = 7
                        },
                        new
                        {
                            Id = 8,
                            Color = 2,
                            Number = 8
                        },
                        new
                        {
                            Id = 9,
                            Color = 2,
                            Number = 9
                        },
                        new
                        {
                            Id = 10,
                            Color = 4,
                            Number = 10
                        },
                        new
                        {
                            Id = 11,
                            Color = 3,
                            Number = 11
                        },
                        new
                        {
                            Id = 12,
                            Color = 1,
                            Number = 12
                        });
                });
#pragma warning restore 612, 618
        }
    }
}