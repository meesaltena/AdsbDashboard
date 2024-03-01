﻿// <auto-generated />
using System;
using AdsbMudBlazor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AdsbMudBlazor.Migrations
{
    [DbContext(typeof(FlightDbContext))]
    partial class FlightDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("AdsbMudBlazor.Models.Flight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Alt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Callsign")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<double?>("Distance")
                        .HasColumnType("REAL");

                    b.Property<double>("Lat")
                        .HasColumnType("REAL");

                    b.Property<double>("Long")
                        .HasColumnType("REAL");

                    b.Property<string>("ModeS")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Squawk")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ModeS", "Callsign", "Alt", "Squawk", "Lat", "Long", "DateTime")
                        .IsUnique();

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("AdsbMudBlazor.Models.Plane", b =>
                {
                    b.Property<string>("ModeS")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastSeen")
                        .HasColumnType("TEXT");

                    b.HasKey("ModeS");

                    b.ToTable("Planes");
                });
#pragma warning restore 612, 618
        }
    }
}
