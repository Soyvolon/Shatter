﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NitroSharp.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NitroSharp.Migrations
{
    [DbContext(typeof(NSDatabaseModel))]
    [Migration("20201008180650_DailyEconTimer")]
    partial class DailyEconTimer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("NitroSharp.Structures.GuildConfig", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("AllowPublicTriviaGames")
                        .HasColumnType("boolean");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TriviaQuestionLimit")
                        .HasColumnType("integer");

                    b.HasKey("GuildId");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("NitroSharp.Structures.Trivia.TriviaPlayer", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("Points")
                        .HasColumnType("integer");

                    b.Property<int>("QuestionsCorrect")
                        .HasColumnType("integer");

                    b.Property<int>("QuestionsIncorrect")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("TriviaPlayers");
                });

            modelBuilder.Entity("NitroSharp.Structures.Wallet", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("Balance")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastDaily")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Wallets");
                });
#pragma warning restore 612, 618
        }
    }
}
