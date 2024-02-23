﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TokenService.Entities;

#nullable disable

namespace TokenService.Migrations
{
    [DbContext(typeof(BookPurchaseTokenDbContext))]
    partial class BookPurchaseTokenDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TokenService.Entities.BookPurchaseTokenEntity", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.HasKey("UserId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("TokenService.Entities.BookPurchaseTokenHistoryEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OrderId")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<long>("UpdatedBalance")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("History");
                });

            modelBuilder.Entity("TokenService.Entities.BookPurchaseTokenHistoryEntity", b =>
                {
                    b.HasOne("TokenService.Entities.BookPurchaseTokenEntity", "BookPurchaseToken")
                        .WithMany("History")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BookPurchaseToken");
                });

            modelBuilder.Entity("TokenService.Entities.BookPurchaseTokenEntity", b =>
                {
                    b.Navigation("History");
                });
#pragma warning restore 612, 618
        }
    }
}
