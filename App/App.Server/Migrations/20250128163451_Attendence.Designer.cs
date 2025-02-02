﻿// <auto-generated />
using System;
using App.Server.Models.AppData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace App.Server.Migrations
{
    [DbContext(typeof(PlannerNPContext))]
    [Migration("20250128163451_Attendence")]
    partial class Attendence
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("App.Server.Models.AppData.Attendence", b =>
                {
                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("RangerId")
                        .HasColumnType("int");

                    b.Property<TimeOnly?>("From")
                        .HasColumnType("time(6)");

                    b.Property<string>("ReasonOfAbsence")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ReasonOfAbsenceEnum")
                        .HasColumnType("int");

                    b.Property<bool>("Working")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Date", "RangerId");

                    b.HasIndex("RangerId");

                    b.ToTable("Attendences");
                });

            modelBuilder.Entity("App.Server.Models.AppData.District", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Districts");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Lock", b =>
                {
                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.HasKey("Date", "DistrictId");

                    b.HasIndex("DistrictId");

                    b.ToTable("Locks");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Plan", b =>
                {
                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("RangerId")
                        .HasColumnType("int");

                    b.HasKey("Date", "RangerId");

                    b.HasIndex("RangerId");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Ranger", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("DistrictId");

                    b.ToTable("Rangers");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Route", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DistrictId");

                    b.ToTable("Routes");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("DistrictId");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("PlanRoute", b =>
                {
                    b.Property<int>("RoutesId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("PlansDate")
                        .HasColumnType("date");

                    b.Property<int>("PlansRangerId")
                        .HasColumnType("int");

                    b.HasKey("RoutesId", "PlansDate", "PlansRangerId");

                    b.HasIndex("PlansDate", "PlansRangerId");

                    b.ToTable("PlanRoute");
                });

            modelBuilder.Entity("PlanVehicle", b =>
                {
                    b.Property<int>("VehiclesId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("PlansDate")
                        .HasColumnType("date");

                    b.Property<int>("PlansRangerId")
                        .HasColumnType("int");

                    b.HasKey("VehiclesId", "PlansDate", "PlansRangerId");

                    b.HasIndex("PlansDate", "PlansRangerId");

                    b.ToTable("PlanVehicle");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Attendence", b =>
                {
                    b.HasOne("App.Server.Models.AppData.Ranger", "Ranger")
                        .WithMany("Attendences")
                        .HasForeignKey("RangerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ranger");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Lock", b =>
                {
                    b.HasOne("App.Server.Models.AppData.District", "District")
                        .WithMany()
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("District");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Plan", b =>
                {
                    b.HasOne("App.Server.Models.AppData.Ranger", "Ranger")
                        .WithMany("Plans")
                        .HasForeignKey("RangerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ranger");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Ranger", b =>
                {
                    b.HasOne("App.Server.Models.AppData.District", "District")
                        .WithMany("Rangers")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("District");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Route", b =>
                {
                    b.HasOne("App.Server.Models.AppData.District", "District")
                        .WithMany()
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("App.Server.Models.AppData.ControlPlace", "ControlPlace", b1 =>
                        {
                            b1.Property<int>("RouteId")
                                .HasColumnType("int");

                            b1.Property<string>("ControlPlaceDescription")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<string>("ControlTime")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.HasKey("RouteId");

                            b1.ToTable("Routes");

                            b1.WithOwner()
                                .HasForeignKey("RouteId");
                        });

                    b.Navigation("ControlPlace");

                    b.Navigation("District");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Vehicle", b =>
                {
                    b.HasOne("App.Server.Models.AppData.District", null)
                        .WithMany("Vehicles")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PlanRoute", b =>
                {
                    b.HasOne("App.Server.Models.AppData.Route", null)
                        .WithMany()
                        .HasForeignKey("RoutesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Server.Models.AppData.Plan", null)
                        .WithMany()
                        .HasForeignKey("PlansDate", "PlansRangerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PlanVehicle", b =>
                {
                    b.HasOne("App.Server.Models.AppData.Vehicle", null)
                        .WithMany()
                        .HasForeignKey("VehiclesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Server.Models.AppData.Plan", null)
                        .WithMany()
                        .HasForeignKey("PlansDate", "PlansRangerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("App.Server.Models.AppData.District", b =>
                {
                    b.Navigation("Rangers");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("App.Server.Models.AppData.Ranger", b =>
                {
                    b.Navigation("Attendences");

                    b.Navigation("Plans");
                });
#pragma warning restore 612, 618
        }
    }
}
