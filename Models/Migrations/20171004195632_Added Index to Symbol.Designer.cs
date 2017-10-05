﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Models;
using System;

namespace Models.Migrations
{
    [DbContext(typeof(StockReporterContext))]
    [Migration("20171004195632_Added Index to Symbol")]
    partial class AddedIndextoSymbol
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Models.Model.CompanyDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IPOyear");

                    b.Property<string>("Industry");

                    b.Property<bool>("IsExTrdFund");

                    b.Property<bool>("IsMutualFund");

                    b.Property<string>("Sector");

                    b.Property<string>("SecurityName")
                        .IsRequired();

                    b.Property<string>("Symbol")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Symbol")
                        .HasName("idx_Symbol");

                    b.ToTable("CompanyDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
