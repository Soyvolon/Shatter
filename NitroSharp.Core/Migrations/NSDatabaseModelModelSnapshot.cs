﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NitroSharp.Core.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NitroSharp.Core.Migrations
{
    [DbContext(typeof(NSDatabaseModel))]
    partial class NSDatabaseModelModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("NitroSharp.Core.Structures.Guilds.GuildConfig", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("AllowPublicTriviaGames")
                        .HasColumnType("boolean");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TriviaQuestionLimit")
                        .HasColumnType("integer");

                    b.HasKey("GuildId");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("NitroSharp.Core.Structures.Guilds.GuildFilters", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("BypassFilters")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Filters")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("GuildId");

                    b.ToTable("Filters");
                });

            modelBuilder.Entity("NitroSharp.Core.Structures.Guilds.GuildMemberlogs", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("JoinDmMessage")
                        .HasColumnType("text");

                    b.Property<decimal?>("MemberlogChannel")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("GuildId");

                    b.ToTable("Memberlogs");
                });

            modelBuilder.Entity("NitroSharp.Core.Structures.Guilds.GuildModeration", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("ModLogChannel")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("SlowmodeLocks")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserBans")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserMutes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("GuildId");

                    b.ToTable("Moderations");
                });

            modelBuilder.Entity("NitroSharp.Core.Structures.Trivia.TriviaPlayer", b =>
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

            modelBuilder.Entity("NitroSharp.Core.Structures.Wallet", b =>
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

            modelBuilder.Entity("NitroSharp.Core.Structures.Guilds.GuildMemberlogs", b =>
                {
                    b.OwnsOne("NitroSharp.Core.Structures.MemberlogMessage", "JoinMessage", b1 =>
                        {
                            b1.Property<decimal>("GuildMemberlogsGuildId")
                                .HasColumnType("numeric(20,0)");

                            b1.Property<string>("ImageUrl")
                                .HasColumnType("text");

                            b1.Property<bool>("IsEmbed")
                                .HasColumnType("boolean");

                            b1.Property<bool>("IsImage")
                                .HasColumnType("boolean");

                            b1.Property<string>("Message")
                                .HasColumnType("text");

                            b1.HasKey("GuildMemberlogsGuildId");

                            b1.ToTable("Memberlogs");

                            b1.WithOwner()
                                .HasForeignKey("GuildMemberlogsGuildId");
                        });

                    b.OwnsOne("NitroSharp.Core.Structures.MemberlogMessage", "LeaveMessage", b1 =>
                        {
                            b1.Property<decimal>("GuildMemberlogsGuildId")
                                .HasColumnType("numeric(20,0)");

                            b1.Property<string>("ImageUrl")
                                .HasColumnType("text");

                            b1.Property<bool>("IsEmbed")
                                .HasColumnType("boolean");

                            b1.Property<bool>("IsImage")
                                .HasColumnType("boolean");

                            b1.Property<string>("Message")
                                .HasColumnType("text");

                            b1.HasKey("GuildMemberlogsGuildId");

                            b1.ToTable("Memberlogs");

                            b1.WithOwner()
                                .HasForeignKey("GuildMemberlogsGuildId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
