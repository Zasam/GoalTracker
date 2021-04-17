﻿// <auto-generated />
using System;
using GoalTracker.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GoalTracker.Migrations
{
    [DbContext(typeof(GoalTrackerContext))]
    partial class GoalTrackerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("GoalTracker.Entities.Achievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("Experience")
                        .HasColumnType("INTEGER");

                    b.Property<string>("InternalTag")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Unlocked")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("GoalTracker.Entities.Goal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AllTasksCompleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("GoalAppointmentInterval")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GoalTaskCount")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasDueDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<int>("NotificationId")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("NotificationTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("RequestCode")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("ViewCellImageString")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Goals");
                });

            modelBuilder.Entity("GoalTracker.Entities.GoalAppointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Approved")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("GoalId")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("Success")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GoalId");

                    b.ToTable("GoalAppointments");
                });

            modelBuilder.Entity("GoalTracker.Entities.GoalTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Completed")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("GoalId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GoalId");

                    b.ToTable("GoalTasks");
                });

            modelBuilder.Entity("GoalTracker.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GoalTracker.Entities.Achievement", b =>
                {
                    b.HasOne("GoalTracker.Entities.User", "User")
                        .WithMany("Achievements")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GoalTracker.Entities.GoalAppointment", b =>
                {
                    b.HasOne("GoalTracker.Entities.Goal", "Goal")
                        .WithMany("GoalAppointments")
                        .HasForeignKey("GoalId");

                    b.Navigation("Goal");
                });

            modelBuilder.Entity("GoalTracker.Entities.GoalTask", b =>
                {
                    b.HasOne("GoalTracker.Entities.Goal", "Goal")
                        .WithMany("GoalTasks")
                        .HasForeignKey("GoalId");

                    b.Navigation("Goal");
                });

            modelBuilder.Entity("GoalTracker.Entities.Goal", b =>
                {
                    b.Navigation("GoalAppointments");

                    b.Navigation("GoalTasks");
                });

            modelBuilder.Entity("GoalTracker.Entities.User", b =>
                {
                    b.Navigation("Achievements");
                });
#pragma warning restore 612, 618
        }
    }
}
