﻿// <auto-generated />
using DiscordBotTicTacToe.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBotTicTacToe.DAL.Migrations.Migrations
{
    [DbContext(typeof(TicTacToeContext))]
    partial class TicTacToeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DiscordBotTicTacToe.DAL.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Abandons")
                        .HasColumnType("int");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Losses")
                        .HasColumnType("int");

                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Wins")
                        .HasColumnType("int");

                    b.Property<bool>("isBanned")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
