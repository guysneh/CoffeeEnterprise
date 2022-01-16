﻿// <auto-generated />
using System;
using CoffeeStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CoffeeStore.Migrations
{
    [DbContext(typeof(CoffeeStoreContext))]
    [Migration("20220116133845_ModifyCoffeeModel")]
    partial class ModifyCoffeeModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CoffeeStore.Models.Coffee", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ProducedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Coffees");
                });

            modelBuilder.Entity("CoffeeStore.Models.Order", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("CoffeeStore.Models.OrderCoffee", b =>
                {
                    b.Property<Guid>("OrderID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CoffeeID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("OrderID", "CoffeeID");

                    b.HasIndex("CoffeeID")
                        .IsUnique();

                    b.ToTable("OrderCoffees");
                });

            modelBuilder.Entity("CoffeeStore.Models.OrderCoffee", b =>
                {
                    b.HasOne("CoffeeStore.Models.Coffee", "Coffee")
                        .WithMany("OrderCoffees")
                        .HasForeignKey("CoffeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoffeeStore.Models.Order", "Order")
                        .WithMany("OrderCoffees")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Coffee");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("CoffeeStore.Models.Coffee", b =>
                {
                    b.Navigation("OrderCoffees");
                });

            modelBuilder.Entity("CoffeeStore.Models.Order", b =>
                {
                    b.Navigation("OrderCoffees");
                });
#pragma warning restore 612, 618
        }
    }
}