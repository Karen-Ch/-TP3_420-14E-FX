﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Seismoscope.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250406060716_ImigrationAJouteEstLivreACapteur")]
    partial class ImigrationAJouteEstLivreACapteur
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("Seismoscope.Model.Capteur", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateInstallation")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EstDesactive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EstLivre")
                        .HasColumnType("INTEGER");

                    b.Property<double>("FrequenceCollecte")
                        .HasColumnType("REAL");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("SeuilAlerte")
                        .HasColumnType("REAL");

                    b.Property<int>("StationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Statut")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("StationId");

                    b.ToTable("Capteurs");
                });

            modelBuilder.Entity("Seismoscope.Model.Station", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateInstallation")
                        .HasColumnType("TEXT");

                    b.Property<string>("Etat")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Localisation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Responsable")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Stations");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MotDePasse")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NomUtilisateur")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Prenom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("StationId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Seismoscope.Model.Capteur", b =>
                {
                    b.HasOne("Seismoscope.Model.Station", "Station")
                        .WithMany("Capteurs")
                        .HasForeignKey("StationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Station");
                });

            modelBuilder.Entity("Seismoscope.Model.Station", b =>
                {
                    b.Navigation("Capteurs");
                });
#pragma warning restore 612, 618
        }
    }
}
