﻿// <auto-generated />
using System;
using MamisSolidarias.Infrastructure.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    [DbContext(typeof(CampaignsDbContext))]
    partial class CampaignsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos.AbrigaditosCampaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CommunityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Edition")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("FundraiserGoal")
                        .HasColumnType("numeric");

                    b.Property<string>("Provider")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Edition", "CommunityId")
                        .IsUnique();

                    b.ToTable("AbrigaditosCampaigns");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos.AbrigaditosParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BeneficiaryGender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("BeneficiaryId")
                        .HasColumnType("integer");

                    b.Property<string>("BeneficiaryName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CampaignId")
                        .HasColumnType("integer");

                    b.Property<string>("ShirtSize")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BeneficiaryName");

                    b.HasIndex("CampaignId");

                    b.ToTable("AbrigaditosParticipants");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.JuntosCampaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CommunityId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Edition")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<decimal>("FundraiserGoal")
                        .HasColumnType("numeric");

                    b.Property<string>("Provider")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.HasIndex("Edition", "CommunityId")
                        .IsUnique();

                    b.ToTable("JuntosCampaigns");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.JuntosParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BeneficiaryId")
                        .HasColumnType("integer");

                    b.Property<int>("CampaignId")
                        .HasColumnType("integer");

                    b.Property<string>("DonationDropOffPoint")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int?>("DonationType")
                        .HasColumnType("integer");

                    b.Property<int?>("DonorId")
                        .HasColumnType("integer");

                    b.Property<string>("DonorName")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<int?>("ShoeSize")
                        .HasMaxLength(20)
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.ToTable("JuntosParticipants");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.MochiCampaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CommunityId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Edition")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("Provider")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.HasIndex("Edition", "CommunityId")
                        .IsUnique();

                    b.ToTable("MochiCampaigns");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.MochiParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BeneficiaryGender")
                        .HasColumnType("integer");

                    b.Property<int>("BeneficiaryId")
                        .HasColumnType("integer");

                    b.Property<string>("BeneficiaryName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("CampaignId")
                        .HasColumnType("integer");

                    b.Property<string>("DonationDropOffLocation")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<Guid?>("DonationId")
                        .HasColumnType("uuid");

                    b.Property<int?>("DonationType")
                        .HasColumnType("integer");

                    b.Property<int?>("DonorId")
                        .HasColumnType("integer");

                    b.Property<string>("DonorName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int?>("SchoolCycle")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.HasIndex("DonationId")
                        .IsUnique();

                    b.HasIndex("BeneficiaryId", "DonorId", "CampaignId")
                        .IsUnique();

                    b.ToTable("MochiParticipants");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos.AbrigaditosCampaign", b =>
                {
                    b.OwnsMany("MamisSolidarias.Infrastructure.Campaigns.Models.Base.CampaignDonation", "Donations", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .HasColumnType("uuid");

                            b1.Property<int>("CampaignId")
                                .HasColumnType("integer");

                            b1.HasKey("Id");

                            b1.HasIndex("CampaignId");

                            b1.ToTable("AbrigaditosDonations", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("CampaignId");
                        });

                    b.Navigation("Donations");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos.AbrigaditosParticipant", b =>
                {
                    b.HasOne("MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos.AbrigaditosCampaign", null)
                        .WithMany("Participants")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.JuntosCampaign", b =>
                {
                    b.OwnsMany("MamisSolidarias.Infrastructure.Campaigns.Models.Base.CampaignDonation", "Donations", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .HasColumnType("uuid");

                            b1.Property<int>("CampaignId")
                                .HasColumnType("integer");

                            b1.HasKey("Id");

                            b1.HasIndex("CampaignId");

                            b1.ToTable("JuntosDonations", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("CampaignId");
                        });

                    b.Navigation("Donations");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.JuntosParticipant", b =>
                {
                    b.HasOne("MamisSolidarias.Infrastructure.Campaigns.Models.JuntosCampaign", "Campaign")
                        .WithMany("Participants")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.MochiParticipant", b =>
                {
                    b.HasOne("MamisSolidarias.Infrastructure.Campaigns.Models.MochiCampaign", "Campaign")
                        .WithMany("Participants")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos.AbrigaditosCampaign", b =>
                {
                    b.Navigation("Participants");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.JuntosCampaign", b =>
                {
                    b.Navigation("Participants");
                });

            modelBuilder.Entity("MamisSolidarias.Infrastructure.Campaigns.Models.MochiCampaign", b =>
                {
                    b.Navigation("Participants");
                });
#pragma warning restore 612, 618
        }
    }
}
