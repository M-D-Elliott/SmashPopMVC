﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using SmashPopMVC.Data;
using SmashPopMVC.Data.Models;
using System;

namespace SmashPopMVC.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190613221105_Added deleted bool to comments.")]
    partial class Addeddeletedbooltocomments
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int?>("AltID");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<int?>("FavoriteSmashGameID");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<int?>("MainID");

                    b.Property<DateTime>("MemberSince");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PartnerId");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("ShortName");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("AltID");

                    b.HasIndex("FavoriteSmashGameID");

                    b.HasIndex("MainID");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("PartnerId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Character", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageName")
                        .HasColumnType("varchar(34)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(30)");

                    b.Property<int?>("OriginID");

                    b.Property<decimal>("Popularity")
                        .HasColumnType("decimal(5,2)");

                    b.Property<string>("SmashID")
                        .HasColumnType("varchar(4)");

                    b.Property<int>("SmashOriginID");

                    b.Property<int>("SmashPopAlts");

                    b.Property<int>("SmashPopMains");

                    b.Property<string>("Tier")
                        .HasColumnType("varchar(4)");

                    b.HasKey("ID");

                    b.HasIndex("OriginID");

                    b.HasIndex("SmashOriginID");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Comment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("Deleted");

                    b.Property<string>("PosteeID");

                    b.Property<string>("PosterId");

                    b.Property<int?>("ReplyToID");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("ID");

                    b.HasIndex("PosteeID");

                    b.HasIndex("PosterId");

                    b.HasIndex("ReplyToID");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Friend", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("BecameFriendsTime");

                    b.Property<int>("FriendRequestFlag");

                    b.Property<DateTime?>("RequestTime");

                    b.Property<string>("RequestedById");

                    b.Property<string>("RequestedToId");

                    b.HasKey("ID");

                    b.HasIndex("RequestedById");

                    b.HasIndex("RequestedToId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Game", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("ReleaseDate");

                    b.Property<string>("SubTitle")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(20)");

                    b.HasKey("ID");

                    b.ToTable("Games");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Game");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Tally", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Month");

                    b.Property<string>("Year");

                    b.HasKey("ID");

                    b.ToTable("Tallies");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Vote", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<int?>("FlavorOfTheMonthID");

                    b.Property<int?>("LeastDifficultID");

                    b.Property<int>("MostDifficultID");

                    b.Property<int?>("MostPowerfulID");

                    b.Property<int>("TallyID");

                    b.Property<string>("VoterID");

                    b.HasKey("ID");

                    b.HasIndex("FlavorOfTheMonthID");

                    b.HasIndex("LeastDifficultID");

                    b.HasIndex("MostDifficultID");

                    b.HasIndex("MostPowerfulID");

                    b.HasIndex("TallyID");

                    b.HasIndex("VoterID");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.OriginGame", b =>
                {
                    b.HasBaseType("SmashPopMVC.Data.Models.Game");


                    b.ToTable("OriginGame");

                    b.HasDiscriminator().HasValue("OriginGame");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.SmashGame", b =>
                {
                    b.HasBaseType("SmashPopMVC.Data.Models.Game");


                    b.ToTable("SmashGame");

                    b.HasDiscriminator().HasValue("SmashGame");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.ApplicationUser", b =>
                {
                    b.HasOne("SmashPopMVC.Data.Models.Character", "Alt")
                        .WithMany()
                        .HasForeignKey("AltID");

                    b.HasOne("SmashPopMVC.Data.Models.Game", "FavoriteSmashGame")
                        .WithMany()
                        .HasForeignKey("FavoriteSmashGameID");

                    b.HasOne("SmashPopMVC.Data.Models.Character", "Main")
                        .WithMany()
                        .HasForeignKey("MainID");

                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser", "Partner")
                        .WithMany()
                        .HasForeignKey("PartnerId");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Character", b =>
                {
                    b.HasOne("SmashPopMVC.Data.Models.OriginGame", "Origin")
                        .WithMany("Characters")
                        .HasForeignKey("OriginID");

                    b.HasOne("SmashPopMVC.Data.Models.SmashGame", "SmashOrigin")
                        .WithMany("Characters")
                        .HasForeignKey("SmashOriginID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Comment", b =>
                {
                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser", "Postee")
                        .WithMany("Comments")
                        .HasForeignKey("PosteeID");

                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser", "Poster")
                        .WithMany()
                        .HasForeignKey("PosterId");

                    b.HasOne("SmashPopMVC.Data.Models.Comment", "ReplyTo")
                        .WithMany("Replies")
                        .HasForeignKey("ReplyToID");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Friend", b =>
                {
                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser", "RequestedBy")
                        .WithMany("SentFriendRequests")
                        .HasForeignKey("RequestedById");

                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser", "RequestedTo")
                        .WithMany("ReceievedFriendRequests")
                        .HasForeignKey("RequestedToId");
                });

            modelBuilder.Entity("SmashPopMVC.Data.Models.Vote", b =>
                {
                    b.HasOne("SmashPopMVC.Data.Models.Character", "FlavorOfTheMonth")
                        .WithMany("FlavorOfTheMonthVotes")
                        .HasForeignKey("FlavorOfTheMonthID");

                    b.HasOne("SmashPopMVC.Data.Models.Character", "LeastDifficult")
                        .WithMany("LeastDifficultVotes")
                        .HasForeignKey("LeastDifficultID");

                    b.HasOne("SmashPopMVC.Data.Models.Character", "MostDifficult")
                        .WithMany("MostDifficultVotes")
                        .HasForeignKey("MostDifficultID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmashPopMVC.Data.Models.Character", "MostPowerful")
                        .WithMany("MostPowerfulVotes")
                        .HasForeignKey("MostPowerfulID");

                    b.HasOne("SmashPopMVC.Data.Models.Tally", "Tally")
                        .WithMany("Votes")
                        .HasForeignKey("TallyID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmashPopMVC.Data.Models.ApplicationUser", "Voter")
                        .WithMany("Votes")
                        .HasForeignKey("VoterID");
                });
#pragma warning restore 612, 618
        }
    }
}
