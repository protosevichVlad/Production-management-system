﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductionManagementSystem.Core.Data.EF;

namespace ProductionManagementSystem.Core.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20220405175255_AddEntityExt")]
    partial class AddEntityExt
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.EntityInProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Designator")
                        .HasColumnType("longtext");

                    b.Property<string>("PartNumber")
                        .HasColumnType("longtext");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("AltiumDB_EntityInProjects");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.Projects.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AssemblyDrawingPath")
                        .HasColumnType("longtext");

                    b.Property<string>("BOMFilePath")
                        .HasColumnType("longtext");

                    b.Property<string>("CircuitDiagramPath")
                        .HasColumnType("longtext");

                    b.Property<string>("ImagePath")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ReportDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ThreeDModelPath")
                        .HasColumnType("longtext");

                    b.Property<string>("Variant")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("AltiumDB_Projects");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.Table", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("FootprintPath")
                        .HasColumnType("longtext");

                    b.Property<string>("LibraryPath")
                        .HasColumnType("longtext");

                    b.Property<string>("TableName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("DatabaseTables");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.TableColumn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ColumnName")
                        .HasColumnType("longtext");

                    b.Property<int>("DatabaseOrder")
                        .HasColumnType("int");

                    b.Property<int>("DatabaseTableId")
                        .HasColumnType("int");

                    b.Property<bool>("Display")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("TableId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TableId");

                    b.ToTable("TableColumns");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.ToDoNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Completed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("CompletedById")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("CompletedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CompletedById");

                    b.HasIndex("CreatedById");

                    b.ToTable("AltiumDB_ToDoNotes");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Components.Design", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("ShortDescription")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Designs");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Components.Montage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Corpus")
                        .HasColumnType("longtext");

                    b.Property<string>("Explanation")
                        .HasColumnType("longtext");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nominal")
                        .HasColumnType("longtext");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Components");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Devices.DesignInDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ComponentId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int?>("DesignId")
                        .HasColumnType("int");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DesignId");

                    b.ToTable("DeviceDesignTemplates");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Devices.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Devices.MontageInDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ComponentId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int?>("MontageId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MontageId");

                    b.ToTable("DeviceComponentsTemplates");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.ElementsDifference.ElementDifference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Difference")
                        .HasColumnType("int");

                    b.Property<int>("ElementId")
                        .HasColumnType("int");

                    b.Property<int>("ElementType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ElementDifferences");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Entity", b =>
                {
                    b.Property<int>("KeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("PartNumber")
                        .HasColumnType("longtext");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("TableId")
                        .HasColumnType("int");

                    b.HasKey("KeyId");

                    b.HasIndex("TableId");

                    b.ToTable("Entities");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Logs.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("DesignId")
                        .HasColumnType("int");

                    b.Property<int?>("DesignSupplyRequestId")
                        .HasColumnType("int");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("longtext");

                    b.Property<int?>("MontageId")
                        .HasColumnType("int");

                    b.Property<int?>("MontageSupplyRequestId")
                        .HasColumnType("int");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Orders.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Customer")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DateStart")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.SupplyRequests.DesignSupplyRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<int>("ComponentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("DesignId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DesiredDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("StatusSupply")
                        .HasColumnType("int");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("DesignId");

                    b.HasIndex("TaskId");

                    b.HasIndex("UserId");

                    b.ToTable("DesignsSupplyRequests");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.SupplyRequests.MontageSupplyRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<int>("ComponentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DesiredDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("MontageId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("StatusSupply")
                        .HasColumnType("int");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("MontageId");

                    b.HasIndex("TaskId");

                    b.HasIndex("UserId");

                    b.ToTable("ComponentsSupplyRequests");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Tasks.ObtainedDesign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ComponentId")
                        .HasColumnType("int");

                    b.Property<int?>("DesignId")
                        .HasColumnType("int");

                    b.Property<int>("Obtained")
                        .HasColumnType("int");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DesignId");

                    b.HasIndex("TaskId");

                    b.ToTable("ObtainedDesigns");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Tasks.ObtainedMontage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ComponentId")
                        .HasColumnType("int");

                    b.Property<int?>("MontageId")
                        .HasColumnType("int");

                    b.Property<int>("Obtained")
                        .HasColumnType("int");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MontageId");

                    b.HasIndex("TaskId");

                    b.ToTable("ObtainedComponents");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Tasks.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("OrderId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.EntityInProject", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.AltiumDB.Projects.Project", "Project")
                        .WithMany("Entities")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.TableColumn", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.AltiumDB.Table", "Table")
                        .WithMany("TableColumns")
                        .HasForeignKey("TableId");

                    b.Navigation("Table");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.ToDoNote", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", "CompletedBy")
                        .WithMany()
                        .HasForeignKey("CompletedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("CompletedBy");

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Devices.DesignInDevice", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Components.Design", "Design")
                        .WithMany()
                        .HasForeignKey("DesignId");

                    b.Navigation("Design");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Devices.MontageInDevice", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Components.Montage", "Montage")
                        .WithMany()
                        .HasForeignKey("MontageId");

                    b.Navigation("Montage");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Entity", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.AltiumDB.Table", "Table")
                        .WithMany()
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Table");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Logs.Log", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.SupplyRequests.DesignSupplyRequest", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Components.Design", "Design")
                        .WithMany()
                        .HasForeignKey("DesignId");

                    b.HasOne("ProductionManagementSystem.Core.Models.Tasks.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId");

                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Design");

                    b.Navigation("Task");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.SupplyRequests.MontageSupplyRequest", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Components.Montage", "Montage")
                        .WithMany()
                        .HasForeignKey("MontageId");

                    b.HasOne("ProductionManagementSystem.Core.Models.Tasks.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId");

                    b.HasOne("ProductionManagementSystem.Core.Models.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Montage");

                    b.Navigation("Task");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Tasks.ObtainedDesign", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Components.Design", "Design")
                        .WithMany()
                        .HasForeignKey("DesignId");

                    b.HasOne("ProductionManagementSystem.Core.Models.Tasks.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Design");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Tasks.ObtainedMontage", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Components.Montage", "Montage")
                        .WithMany()
                        .HasForeignKey("MontageId");

                    b.HasOne("ProductionManagementSystem.Core.Models.Tasks.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Montage");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Tasks.Task", b =>
                {
                    b.HasOne("ProductionManagementSystem.Core.Models.Devices.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProductionManagementSystem.Core.Models.Orders.Order", "Order")
                        .WithMany("Tasks")
                        .HasForeignKey("OrderId");

                    b.Navigation("Device");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.Projects.Project", b =>
                {
                    b.Navigation("Entities");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.AltiumDB.Table", b =>
                {
                    b.Navigation("TableColumns");
                });

            modelBuilder.Entity("ProductionManagementSystem.Core.Models.Orders.Order", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
