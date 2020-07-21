using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DataSystem.Models;

namespace DataSystem.Migrations
{
    [DbContext(typeof(WebNutContext))]
    [Migration("20180202132552_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("checkcompleteness", b =>
                {
                    b.Property<string>("NMRID")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.Property<string>("District")
                        .HasMaxLength(50);

                    b.Property<int>("FacilityID");

                    b.Property<string>("FacilityName");

                    b.Property<int>("IPDSAM_stock_submission");

                    b.Property<int>("IPDSAM_submission");

                    b.Property<int>("IYCF_submission");

                    b.Property<string>("Implementer")
                        .HasMaxLength(20);

                    b.Property<int>("MNS_submission");

                    b.Property<int>("Month");

                    b.Property<int>("OPDMAM_stock_submission");

                    b.Property<int>("OPDMAM_submission");

                    b.Property<int>("OPDSAM_stock_submission");

                    b.Property<int>("OPDSAM_submission");

                    b.Property<string>("Province")
                        .HasMaxLength(50);

                    b.Property<int?>("StatusId");

                    b.Property<string>("UserName");

                    b.Property<int>("Year");

                    b.Property<string>("message")
                        .HasMaxLength(500);

                    b.HasKey("NMRID");

                    b.ToTable("checkcompleteness");
                });

            modelBuilder.Entity("DataSystem.Models.Districts", b =>
                {
                    b.Property<string>("DistCode")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2(0)");

                    b.Property<string>("DistName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("DistNameDari")
                        .HasMaxLength(50);

                    b.Property<string>("DistNamePashto")
                        .HasMaxLength(255);

                    b.Property<string>("ProvCode")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("DistCode")
                        .HasName("Districts$PrimaryKey");

                    b.HasIndex("DistName")
                        .HasName("Districts$DistName");

                    b.HasIndex("DistNameDari")
                        .HasName("Districts$DistNameDari");

                    b.HasIndex("ProvCode")
                        .HasName("Districts$PROV_34_ID");

                    b.ToTable("Districts");
                });

            modelBuilder.Entity("DataSystem.Models.FacilityInfo", b =>
                {
                    b.Property<int>("FacilityId")
                        .HasColumnName("FacilityID");

                    b.Property<string>("ActiveStatus")
                        .HasMaxLength(10);

                    b.Property<DateTime?>("DateEstablished")
                        .HasColumnType("datetime2(0)");

                    b.Property<string>("DistCode")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("FacilityName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("FacilityNameDari")
                        .HasMaxLength(255);

                    b.Property<string>("FacilityNamePashto")
                        .HasMaxLength(255);

                    b.Property<int?>("FacilityType")
                        .IsRequired();

                    b.Property<double?>("Gpslattitude")
                        .HasColumnName("GPSLattitude");

                    b.Property<double?>("Gpslongtitude")
                        .HasColumnName("GPSLongtitude");

                    b.Property<string>("Implementer")
                        .IsRequired();

                    b.Property<double?>("Lat")
                        .HasColumnName("LAT");

                    b.Property<string>("Location")
                        .HasMaxLength(100);

                    b.Property<string>("LocationDari")
                        .HasMaxLength(100);

                    b.Property<string>("LocationPashto")
                        .HasMaxLength(100);

                    b.Property<double?>("Lon")
                        .HasColumnName("LON");

                    b.Property<string>("SubImplementer")
                        .HasMaxLength(255);

                    b.Property<string>("ViliCode")
                        .HasMaxLength(255);

                    b.HasKey("FacilityId")
                        .HasName("FacilityInfo$PrimaryKey");

                    b.HasIndex("DistCode")
                        .HasName("FacilityInfo$DistCode");

                    b.HasIndex("FacilityType");

                    b.HasIndex("ViliCode")
                        .HasName("FacilityInfo$GOCode");

                    b.ToTable("FacilityInfo");
                });

            modelBuilder.Entity("DataSystem.Models.FacilityTypes", b =>
                {
                    b.Property<int>("FacTypeCode");

                    b.Property<string>("FacType")
                        .HasMaxLength(50);

                    b.Property<int?>("FacTypeCatCode");

                    b.Property<string>("FacTypeDari")
                        .HasMaxLength(50);

                    b.Property<string>("FacTypePashto")
                        .HasMaxLength(50);

                    b.HasKey("FacTypeCode")
                        .HasName("FacilityTypes$PrimaryKey");

                    b.HasIndex("FacTypeCatCode")
                        .HasName("FacilityTypes$FacTypeCatCode");

                    b.ToTable("FacilityTypes");
                });

            modelBuilder.Entity("DataSystem.Models.Implementers", b =>
                {
                    b.Property<int>("ImpCode");

                    b.Property<string>("AfghanistanAddress");

                    b.Property<string>("ImpAcronym")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("ImpName")
                        .HasMaxLength(255);

                    b.Property<string>("ImpNameDari")
                        .HasMaxLength(255);

                    b.Property<string>("ImpNamePashto")
                        .HasMaxLength(255);

                    b.Property<bool>("ImpStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("impStatus")
                        .HasDefaultValueSql("0");

                    b.Property<string>("OtherAddress");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime2(0)");

                    b.HasKey("ImpCode")
                        .HasName("Implementers$PrimaryKey");

                    b.HasIndex("ImpAcronym")
                        .HasName("Implementers$UNOPSCode");

                    b.HasIndex("ImpName")
                        .HasName("Implementers$MRRD_Code");

                    b.ToTable("Implementers");
                });

            modelBuilder.Entity("DataSystem.Models.LkpHfstatus", b =>
                {
                    b.Property<int>("HfactiveStatusId")
                        .HasColumnName("HFActiveStatusID");

                    b.Property<string>("HfstatusDescription")
                        .HasColumnName("HFStatusDescription")
                        .HasMaxLength(50);

                    b.HasKey("HfactiveStatusId")
                        .HasName("PK_lkpHFStatus");

                    b.ToTable("lkpHFStatus");
                });

            modelBuilder.Entity("DataSystem.Models.Mamreq", b =>
                {
                    b.Property<long>("Rid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RID");

                    b.Property<short>("Bhc");

                    b.Property<short>("Chc")
                        .HasColumnName("CHC");

                    b.Property<short>("Dh")
                        .HasColumnName("DH");

                    b.Property<string>("ImpCode");

                    b.Property<short>("Mht")
                        .HasColumnName("MHT");

                    b.Property<short>("Month");

                    b.Property<short>("Ph")
                        .HasColumnName("PH");

                    b.Property<string>("ProvCode")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ReqBy")
                        .HasMaxLength(50);

                    b.Property<short?>("ReqMonth");

                    b.Property<int?>("ReqYear");

                    b.Property<short>("Shc")
                        .HasColumnName("SHC");

                    b.Property<int>("Tenant");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("date");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Year");

                    b.HasKey("Rid")
                        .HasName("PK_MAMReq");

                    b.HasIndex("ProvCode", "ImpCode", "Year", "Month")
                        .IsUnique()
                        .HasName("UQ__MAMReq__98EDC49B9F16E08E");

                    b.ToTable("MAMReq");
                });

            modelBuilder.Entity("DataSystem.Models.MamreqDetails", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Adjustment");

                    b.Property<string>("AdjustmentComment")
                        .HasMaxLength(255);

                    b.Property<int?>("CurrentBalance");

                    b.Property<string>("FormName");

                    b.Property<int?>("NoOfBenificiaries");

                    b.Property<long>("Rid")
                        .HasColumnName("RID");

                    b.Property<int>("SupplyId");

                    b.HasKey("Id");

                    b.HasIndex("SupplyId");

                    b.HasIndex("Rid", "SupplyId", "FormName")
                        .IsUnique()
                        .HasName("UQ__MAMReqDe__3D3297F9B5B94ECB");

                    b.ToTable("MAMReqDetails");
                });

            modelBuilder.Entity("DataSystem.Models.monthlysubmission", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(24);

                    b.Property<string>("District")
                        .HasMaxLength(20);

                    b.Property<int>("FacilityID");

                    b.Property<string>("FacilityName");

                    b.Property<int>("M1");

                    b.Property<int>("M10");

                    b.Property<int>("M11");

                    b.Property<int>("M12");

                    b.Property<int>("M2");

                    b.Property<int>("M3");

                    b.Property<int>("M4");

                    b.Property<int>("M5");

                    b.Property<int>("M6");

                    b.Property<int>("M7");

                    b.Property<int>("M8");

                    b.Property<int>("M9");

                    b.Property<string>("Province")
                        .HasMaxLength(50);

                    b.Property<string>("UserName");

                    b.Property<int>("Year");

                    b.HasKey("ID");

                    b.ToTable("monthlysubmission");
                });

            modelBuilder.Entity("DataSystem.Models.Nmr", b =>
                {
                    b.Property<string>("Nmrid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<int?>("BoysScreened");

                    b.Property<string>("Commen")
                        .HasColumnName("commen");

                    b.Property<int>("FacilityId")
                        .HasColumnName("FacilityID");

                    b.Property<int?>("FacilityType");

                    b.Property<int?>("Flanumber")
                        .HasColumnName("FLANumber");

                    b.Property<int?>("GirlsScreened");

                    b.Property<int?>("HfactiveStatusId")
                        .HasColumnName("HFActiveStatusID");

                    b.Property<int?>("IalsKwashiorkor")
                        .HasColumnName("IALS_Kwashiorkor");

                    b.Property<int?>("IalsMarasmus")
                        .HasColumnName("IALS_Marasmus");

                    b.Property<int?>("IawgKwashiorkor")
                        .HasColumnName("IAWG_Kwashiorkor");

                    b.Property<int?>("IawgMarasmus")
                        .HasColumnName("IAWG_Marasmus");

                    b.Property<string>("Implementer")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("IpdAdmissionsByChws");

                    b.Property<int?>("IpdRutfstockOutWeeks");

                    b.Property<int?>("MamAddminsionByChws");

                    b.Property<int?>("MamRusfstockoutWeeks");

                    b.Property<int>("Month");

                    b.Property<int?>("OalsKwashiorkor")
                        .HasColumnName("OALS_Kwashiorkor");

                    b.Property<int?>("OalsMarasmus")
                        .HasColumnName("OALS_Marasmus");

                    b.Property<int?>("OawgKwashiorkor")
                        .HasColumnName("OAWG_Kwashiorkor");

                    b.Property<int?>("OawgMarasmus")
                        .HasColumnName("OAWG_Marasmus");

                    b.Property<int?>("OpdAdmissionsByChws");

                    b.Property<int?>("OpdRutfstockOutWeeks");

                    b.Property<DateTime?>("OpeningDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("Plwreported");

                    b.Property<string>("PreparedBy")
                        .HasMaxLength(255);

                    b.Property<int?>("SfpAls")
                        .HasColumnName("SFP_ALS");

                    b.Property<int?>("SfpAwg")
                        .HasColumnName("SFP_AWG");

                    b.Property<int?>("StatusId")
                        .HasColumnName("StatusID");

                    b.Property<int>("Tenant");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.Property<int>("Year");

                    b.Property<bool>("isHumanitarian");

                    b.Property<int>("mMonth");

                    b.Property<int>("mYear");

                    b.Property<string>("message");

                    b.HasKey("Nmrid");

                    b.HasIndex("FacilityId")
                        .HasName("NutritionMonthlyReport$FacilityInfoNutritionMonthlyReport");

                    b.HasIndex("HfactiveStatusId");

                    b.HasIndex("StatusId");

                    b.ToTable("NMR");
                });

            modelBuilder.Entity("DataSystem.Models.Provinces", b =>
                {
                    b.Property<string>("ProvCode")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<int?>("AGHCHOCode");

                    b.Property<int?>("AIMSCode");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2(0)");

                    b.Property<string>("ProvName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("ProveNameDari")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ProveNamePashto")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("ProvCode")
                        .HasName("Provinces$PrimaryKey");

                    b.HasIndex("ProvName")
                        .IsUnique()
                        .HasName("Provinces$ProvName");

                    b.HasIndex("ProveNameDari")
                        .IsUnique()
                        .HasName("Provinces$ProveNameDari");

                    b.HasIndex("ProveNamePashto")
                        .IsUnique()
                        .HasName("Provinces$ProveNamePashtu");

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("DataSystem.Models.Samreq", b =>
                {
                    b.Property<long>("Rid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RID");

                    b.Property<short>("Bhc");

                    b.Property<short>("Chc")
                        .HasColumnName("CHC");

                    b.Property<short>("Dh")
                        .HasColumnName("DH");

                    b.Property<string>("ImpCode")
                        .IsRequired();

                    b.Property<short>("Mht")
                        .HasColumnName("MHT");

                    b.Property<short>("Month");

                    b.Property<short?>("MonthFrom");

                    b.Property<short?>("MonthTo");

                    b.Property<short>("Ph")
                        .HasColumnName("PH");

                    b.Property<string>("ProvCode")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ReqBy")
                        .HasMaxLength(50);

                    b.Property<short>("Shc")
                        .HasColumnName("SHC");

                    b.Property<int>("Tenant");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("date");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Year");

                    b.Property<short?>("YearFrom");

                    b.Property<short?>("YearTo");

                    b.HasKey("Rid")
                        .HasName("PK_SAMReq");

                    b.HasIndex("ProvCode", "ImpCode", "Year", "Month")
                        .IsUnique()
                        .HasName("UQ__SAMReq__E61DE2A759B61604");

                    b.ToTable("SAMReq");
                });

            modelBuilder.Entity("DataSystem.Models.SamreqDetails", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Adjustment");

                    b.Property<string>("AdjustmentComment")
                        .HasMaxLength(255);

                    b.Property<int?>("CurrentBalance");

                    b.Property<string>("FormName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int?>("O6");

                    b.Property<long>("Rid")
                        .HasColumnName("RID");

                    b.Property<int>("SupplyId");

                    b.Property<int?>("U6");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("date");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("SupplyId");

                    b.HasIndex("Rid", "SupplyId", "FormName")
                        .IsUnique()
                        .HasName("UQ__SAMReqDe__12B320735C79634D");

                    b.ToTable("SAMReqDetails");
                });

            modelBuilder.Entity("DataSystem.Models.TblFeedback", b =>
                {
                    b.Property<int>("FedId")
                        .HasColumnName("FedID");

                    b.Property<string>("Comments");

                    b.Property<int?>("FormId")
                        .HasColumnName("FormID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.HasKey("FedId")
                        .HasName("PK_tblFeedback");

                    b.HasIndex("FormId");

                    b.HasIndex("Nmrid");

                    b.ToTable("tblFeedback");
                });

            modelBuilder.Entity("DataSystem.Models.TblFstock", b =>
                {
                    b.Property<int>("StockId")
                        .HasColumnName("stockID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<int?>("ExpectedRecepients");

                    b.Property<int?>("Losses");

                    b.Property<int?>("OpeningBalance");

                    b.Property<int?>("QuantityDistributed");

                    b.Property<int?>("QuantityReceived");

                    b.Property<int?>("QuantityReferin");

                    b.Property<int?>("QuantityReturned");

                    b.Property<int?>("QuantityTransferred");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.HasKey("StockId", "Nmrid")
                        .HasName("tblFstock$PrimaryKey");

                    b.HasIndex("Nmrid")
                        .HasName("tblFstock$NMRID");

                    b.HasIndex("StockId")
                        .HasName("tblFstock$stockID1");

                    b.ToTable("tblFstock");
                });

            modelBuilder.Entity("DataSystem.Models.TblIycf", b =>
                {
                    b.Property<int>("Iycfid")
                        .HasColumnName("IYCFID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<int?>("Firstvisit");

                    b.Property<int?>("MChild524months")
                        .HasColumnName("mChild524months");

                    b.Property<int?>("MChildU5months")
                        .HasColumnName("mChildU5months");

                    b.Property<int?>("Pregnanatwomen")
                        .HasColumnName("pregnanatwomen");

                    b.Property<int?>("ReferIn");

                    b.Property<int?>("ReferOut");

                    b.Property<int?>("Revisit");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.HasKey("Iycfid", "Nmrid")
                        .HasName("tblIYCF$PrimaryKey");

                    b.HasIndex("Iycfid")
                        .HasName("tblIYCF$InputID");

                    b.HasIndex("Nmrid")
                        .HasName("tblIYCF${AC5A7038-0E9D-4C9A-A138-0D220C0EE76E}");

                    b.ToTable("tblIYCF");
                });

            modelBuilder.Entity("DataSystem.Models.TblkpStatus", b =>
                {
                    b.Property<int>("StatusId")
                        .HasColumnName("StatusID");

                    b.Property<string>("StatusDescription")
                        .HasMaxLength(50);

                    b.HasKey("StatusId")
                        .HasName("PK_tblkpStatus");

                    b.ToTable("tblkpStatus");
                });

            modelBuilder.Entity("DataSystem.Models.TblMam", b =>
                {
                    b.Property<int>("Mamid")
                        .HasColumnName("MAMID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<int?>("Absents");

                    b.Property<int?>("Cured");

                    b.Property<int?>("Deaths");

                    b.Property<int?>("Defaulters");

                    b.Property<int?>("Muac12")
                        .HasColumnName("MUAC12");

                    b.Property<int?>("Muac23")
                        .HasColumnName("MUAC23");

                    b.Property<int?>("NonCured");

                    b.Property<int?>("ReferIn");

                    b.Property<int?>("TFemale")
                        .HasColumnName("tFemale");

                    b.Property<int?>("TMale")
                        .HasColumnName("tMale");

                    b.Property<int?>("Totalbegin")
                        .HasColumnName("totalbegin");

                    b.Property<int?>("Transfers");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.Property<int?>("Zscore23");

                    b.HasKey("Mamid", "Nmrid")
                        .HasName("tblSFP$PrimaryKey");

                    b.HasIndex("Mamid")
                        .HasName("tblSFP$SFPID");

                    b.HasIndex("Nmrid")
                        .HasName("tblSFP$NMRID");

                    b.ToTable("tblMAM");
                });

            modelBuilder.Entity("DataSystem.Models.TblMn", b =>
                {
                    b.Property<int>("Mnid")
                        .HasColumnName("MNID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<string>("Remarks");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.Property<int?>("chu2f")
                        .HasColumnName("chu2f");

                    b.Property<int?>("chu2m")
                        .HasColumnName("chu2m");

                    b.Property<int?>("refbyCHW")
                        .HasColumnName("refbyChw");

                    b.HasKey("Mnid", "Nmrid")
                        .HasName("tblMN$PrimaryKey");

                    b.HasIndex("Mnid")
                        .HasName("tblMN${58F5E5B9-3552-4441-8E08-55B12448DD25}");

                    b.HasIndex("Nmrid")
                        .HasName("tblMN$NMRID");

                    b.ToTable("tblMN");
                });

            modelBuilder.Entity("DataSystem.Models.TblOtp", b =>
                {
                    b.Property<int>("Otpid")
                        .HasColumnName("OTPID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<int?>("Cured");

                    b.Property<int?>("Death");

                    b.Property<int?>("Default");

                    b.Property<int?>("Defaultreturn")
                        .HasColumnName("defaultreturn");

                    b.Property<int?>("Fromscotp")
                        .HasColumnName("fromscotp");

                    b.Property<int?>("Fromsfp")
                        .HasColumnName("fromsfp");

                    b.Property<int?>("Muac115")
                        .HasColumnName("MUAC115");

                    b.Property<int?>("NonCured");

                    b.Property<int?>("Odema")
                        .HasColumnName("odema");

                    b.Property<int?>("RefOut");

                    b.Property<int?>("TFemale")
                        .HasColumnName("tFemale");

                    b.Property<int?>("TMale")
                        .HasColumnName("tMale");

                    b.Property<int?>("Totalbegin")
                        .HasColumnName("totalbegin");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.Property<int?>("Z3score")
                        .HasColumnName("z3score");

                    b.HasKey("Otpid", "Nmrid")
                        .HasName("tblOTP$PrimaryKey");

                    b.HasIndex("Nmrid")
                        .HasName("tblOTP$NMRID");

                    b.HasIndex("Otpid")
                        .HasName("tblOTP$SFPID");

                    b.ToTable("tblOTP");
                });

            modelBuilder.Entity("DataSystem.Models.TblOtptfu", b =>
                {
                    b.Property<int>("Otptfuid")
                        .HasColumnName("OTPTFUID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<int?>("Cured");

                    b.Property<int?>("Death");

                    b.Property<int?>("Default");

                    b.Property<int?>("Defaultreturn")
                        .HasColumnName("defaultreturn");

                    b.Property<int?>("Fromscotp")
                        .HasColumnName("fromscotp");

                    b.Property<int?>("Fromsfp")
                        .HasColumnName("fromsfp");

                    b.Property<int?>("Muac115")
                        .HasColumnName("MUAC115");

                    b.Property<int?>("NonCured");

                    b.Property<int?>("Odema")
                        .HasColumnName("odema");

                    b.Property<int?>("RefOut");

                    b.Property<int?>("TFemale")
                        .HasColumnName("tFemale");

                    b.Property<int?>("TMale")
                        .HasColumnName("tMale");

                    b.Property<int?>("Totalbegin")
                        .HasColumnName("totalbegin");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.Property<int?>("Z3score")
                        .HasColumnName("z3score");

                    b.HasKey("Otptfuid", "Nmrid")
                        .HasName("tblOTPTFU$PrimaryKey");

                    b.HasIndex("Nmrid")
                        .HasName("tblOTPTFU$NMRID");

                    b.HasIndex("Otptfuid")
                        .HasName("tblOTPTFU$SFPID");

                    b.ToTable("tblOTPTFU");
                });

            modelBuilder.Entity("DataSystem.Models.TblQnr", b =>
                {
                    b.Property<int>("Qnrid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("QNRID");

                    b.Property<string>("Highlights")
                        .HasMaxLength(500);

                    b.Property<int>("Implementer")
                        .HasMaxLength(50);

                    b.Property<string>("IpdsamAdmissionsTrend")
                        .HasColumnName("IPDSAM_AdmissionsTrend")
                        .HasMaxLength(500);

                    b.Property<string>("IpdsamPerformanceTrend")
                        .HasColumnName("IPDSAM_PerformanceTrend")
                        .HasMaxLength(500);

                    b.Property<string>("Iycf")
                        .HasColumnName("IYCF")
                        .HasMaxLength(500);

                    b.Property<string>("Micronutrients")
                        .HasMaxLength(500);

                    b.Property<string>("OpdmamAdmissionsTrend")
                        .HasColumnName("OPDMAM_AdmissionsTrend")
                        .HasMaxLength(500);

                    b.Property<string>("OpdmamPerformanceTrend")
                        .HasColumnName("OPDMAM_PerformanceTrend")
                        .HasMaxLength(500);

                    b.Property<string>("OpdsamAdmissionsTrend")
                        .HasColumnName("OPDSAM_AdmissionsTrend")
                        .HasMaxLength(500);

                    b.Property<string>("OpdsamPerformanceTrend")
                        .HasColumnName("OPDSAM_PerformanceTrend")
                        .HasMaxLength(500);

                    b.Property<string>("Province")
                        .HasMaxLength(50);

                    b.Property<int?>("ReportMonth");

                    b.Property<int?>("ReportYear");

                    b.Property<DateTime?>("ReportingDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("StatusId");

                    b.Property<int>("Tenant");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.Property<string>("message");

                    b.HasKey("Qnrid")
                        .HasName("PK_tblQNR");

                    b.HasIndex("Implementer");

                    b.HasIndex("Province");

                    b.HasIndex("StatusId");

                    b.ToTable("tblQNR");
                });

            modelBuilder.Entity("DataSystem.Models.TblStockIpt", b =>
                {
                    b.Property<int>("SstockId")
                        .HasColumnName("sstockID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<decimal?>("Damaged")
                        .HasColumnType("decimal");

                    b.Property<decimal?>("Expired")
                        .HasColumnType("decimal");

                    b.Property<decimal?>("Loss")
                        .HasColumnType("decimal");

                    b.Property<decimal?>("Openingbalance")
                        .HasColumnType("decimal");

                    b.Property<decimal?>("Received");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<decimal?>("Used")
                        .HasColumnType("decimal");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.HasKey("SstockId", "Nmrid")
                        .HasName("tblSstock$PrimaryKey");

                    b.HasIndex("Nmrid")
                        .HasName("tblSstock$NMRID");

                    b.HasIndex("SstockId")
                        .HasName("tblSstock$stockID1");

                    b.ToTable("tblStock_ipt");
                });

            modelBuilder.Entity("DataSystem.Models.TblStockOtp", b =>
                {
                    b.Property<int>("SstockotpId")
                        .HasColumnName("sstockotpID");

                    b.Property<string>("Nmrid")
                        .HasColumnName("NMRID")
                        .HasMaxLength(100);

                    b.Property<decimal?>("Damaged");

                    b.Property<decimal?>("Expired");

                    b.Property<decimal?>("Loss")
                        .HasColumnType("decimal");

                    b.Property<decimal?>("Openingbalance")
                        .HasColumnType("decimal");

                    b.Property<decimal?>("Received");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<decimal?>("Used");

                    b.Property<string>("UserName")
                        .HasMaxLength(50);

                    b.HasKey("SstockotpId", "Nmrid")
                        .HasName("tblSstock_otp$PrimaryKey");

                    b.HasIndex("Nmrid")
                        .HasName("tblSstock_otp$NMRID");

                    b.HasIndex("SstockotpId")
                        .HasName("tblSstock_otp$stockID1");

                    b.ToTable("tblStock_otp");
                });

            modelBuilder.Entity("DataSystem.Models.Tenant", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id")
                        .HasName("PK_tenant");

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("DataSystem.Models.TlkpForms", b =>
                {
                    b.Property<int>("FormId")
                        .HasColumnName("FormID");

                    b.Property<string>("FormName")
                        .HasMaxLength(100);

                    b.HasKey("FormId")
                        .HasName("PK_tlkpForms");

                    b.ToTable("tlkpForms");
                });

            modelBuilder.Entity("DataSystem.Models.TlkpFstock", b =>
                {
                    b.Property<int>("StockId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("stockID");

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.Property<float>("Buffer");

                    b.Property<decimal?>("DistAmountKg")
                        .HasColumnType("decimal");

                    b.Property<string>("Item")
                        .HasMaxLength(255);

                    b.Property<float>("Zarib");

                    b.HasKey("StockId")
                        .HasName("tlkpFstock$stockID");

                    b.ToTable("tlkpFstock");
                });

            modelBuilder.Entity("DataSystem.Models.TlkpIycf", b =>
                {
                    b.Property<int>("Iycfid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IYCFID");

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.Property<string>("CauseConsultation")
                        .HasColumnName("cause_consultation")
                        .HasMaxLength(255);

                    b.Property<string>("CauseShortName")
                        .HasMaxLength(255);

                    b.HasKey("Iycfid")
                        .HasName("tlkpIYCF$SFPID");

                    b.ToTable("tlkpIYCF");
                });

            modelBuilder.Entity("DataSystem.Models.TlkpMn", b =>
                {
                    b.Property<int>("Mnid")
                        .HasColumnName("MNID");

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.Property<string>("Mnitems")
                        .HasColumnName("MNitems")
                        .HasMaxLength(255);

                    b.HasKey("Mnid")
                        .HasName("tlkpMN$PrimaryKey");

                    b.ToTable("tlkpMN");
                });

            modelBuilder.Entity("DataSystem.Models.TlkpOtptfu", b =>
                {
                    b.Property<int>("Otptfuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OTPTFUID");

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.Property<string>("AgeGroup")
                        .HasMaxLength(255);

                    b.HasKey("Otptfuid")
                        .HasName("tlkpOTPTFU$SFPID");

                    b.ToTable("tlkpOTPTFU");
                });

            modelBuilder.Entity("DataSystem.Models.TlkpSfp", b =>
                {
                    b.Property<int>("Sfpid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SFPID");

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.Property<string>("AgeGroup")
                        .HasMaxLength(255);

                    b.HasKey("Sfpid")
                        .HasName("tlkpSFP$SFPID");

                    b.ToTable("tlkpSFP");
                });

            modelBuilder.Entity("DataSystem.Models.TlkpSstock", b =>
                {
                    b.Property<int>("SstockId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("sstockID");

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("0");

                    b.Property<float>("Buffer");

                    b.Property<float>("IPDSAMZarib");

                    b.Property<string>("Item")
                        .HasMaxLength(255);

                    b.Property<float>("OPDSAMZarib");

                    b.Property<int?>("Persachet");

                    b.HasKey("SstockId")
                        .HasName("tlkpSstock$stockID");

                    b.ToTable("tlkpSstock");
                });

            modelBuilder.Entity("DataSystem.Models.ViewModels.chart.submissionRes", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ExDistricts");

                    b.Property<int>("ExFaciliteis");

                    b.Property<int>("ExProvinces");

                    b.Property<int>("SubDistricts");

                    b.Property<int>("SubFacilities");

                    b.Property<int>("SubOrgs");

                    b.Property<int>("SubProvinces");

                    b.HasKey("ID");

                    b.ToTable("submissionRes");
                });

            modelBuilder.Entity("nmrsubmission", b =>
                {
                    b.Property<string>("NMRID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("District")
                        .HasMaxLength(50);

                    b.Property<int>("FacilityID");

                    b.Property<string>("FacilityName");

                    b.Property<string>("FacilityType");

                    b.Property<int>("IPDSAM_stock_submission");

                    b.Property<int>("IPDSAM_submission");

                    b.Property<int>("IYCF_submission");

                    b.Property<string>("Implementer")
                        .HasMaxLength(20);

                    b.Property<int>("MNS_submission");

                    b.Property<int>("Month");

                    b.Property<int>("OPDMAM_stock_submission");

                    b.Property<int>("OPDMAM_submission");

                    b.Property<int>("OPDSAM_stock_submission");

                    b.Property<int>("OPDSAM_submission");

                    b.Property<string>("ProvCode")
                        .HasMaxLength(2);

                    b.Property<string>("Province")
                        .HasMaxLength(50);

                    b.Property<int>("Year");

                    b.Property<int>("mMonth");

                    b.Property<int>("mYear");

                    b.HasKey("NMRID");

                    b.ToTable("nmrsubmission");
                });

            modelBuilder.Entity("DataSystem.Models.Districts", b =>
                {
                    b.HasOne("DataSystem.Models.Provinces", "ProvCodeNavigation")
                        .WithMany("Districts")
                        .HasForeignKey("ProvCode");
                });

            modelBuilder.Entity("DataSystem.Models.FacilityInfo", b =>
                {
                    b.HasOne("DataSystem.Models.Districts", "DistNavigation")
                        .WithMany()
                        .HasForeignKey("DistCode")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.FacilityTypes", "FacilityTypeNavigation")
                        .WithMany()
                        .HasForeignKey("FacilityType")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSystem.Models.MamreqDetails", b =>
                {
                    b.HasOne("DataSystem.Models.Mamreq", "R")
                        .WithMany("MamreqDetails")
                        .HasForeignKey("Rid")
                        .HasConstraintName("FK_MAMReqDetails_MAMReq")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.TlkpFstock", "SId")
                        .WithMany("MamreqDetails")
                        .HasForeignKey("SupplyId")
                        .HasConstraintName("FK_Mam_supply");
                });

            modelBuilder.Entity("DataSystem.Models.Nmr", b =>
                {
                    b.HasOne("DataSystem.Models.FacilityInfo", "Facility")
                        .WithMany("Nmr")
                        .HasForeignKey("FacilityId")
                        .HasConstraintName("FK_NutritionMonthlyReport_FacilityInfo");

                    b.HasOne("DataSystem.Models.LkpHfstatus", "HfactiveStatus")
                        .WithMany("Nmr")
                        .HasForeignKey("HfactiveStatusId")
                        .HasConstraintName("FK_NMR_lkpHFStatus");

                    b.HasOne("DataSystem.Models.TblkpStatus", "Status")
                        .WithMany("Nmr")
                        .HasForeignKey("StatusId");
                });

            modelBuilder.Entity("DataSystem.Models.SamreqDetails", b =>
                {
                    b.HasOne("DataSystem.Models.Samreq", "R")
                        .WithMany("SamreqDetails")
                        .HasForeignKey("Rid")
                        .HasConstraintName("FK_SAMReqDetails_SAMReq")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.TlkpSstock", "SId")
                        .WithMany("SamreqDetails")
                        .HasForeignKey("SupplyId")
                        .HasConstraintName("FK_tblSstock_tlkpSstock");
                });

            modelBuilder.Entity("DataSystem.Models.TblFeedback", b =>
                {
                    b.HasOne("DataSystem.Models.TlkpForms", "Form")
                        .WithMany("TblFeedback")
                        .HasForeignKey("FormId");

                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany("TblFeedback")
                        .HasForeignKey("Nmrid")
                        .HasConstraintName("FK_tblFeedback_NMR");
                });

            modelBuilder.Entity("DataSystem.Models.TblFstock", b =>
                {
                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany()
                        .HasForeignKey("Nmrid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.TlkpFstock", "Stock")
                        .WithMany("TblFstock")
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSystem.Models.TblIycf", b =>
                {
                    b.HasOne("DataSystem.Models.TlkpIycf", "Iycf")
                        .WithMany("TblIycf")
                        .HasForeignKey("Iycfid");

                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany("TblIycf")
                        .HasForeignKey("Nmrid")
                        .HasConstraintName("tblIYCF${AC5A7038-0E9D-4C9A-A138-0D220C0EE76E}")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSystem.Models.TblMam", b =>
                {
                    b.HasOne("DataSystem.Models.TlkpSfp", "Mam")
                        .WithMany("TblMam")
                        .HasForeignKey("Mamid");

                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany("TblMam")
                        .HasForeignKey("Nmrid")
                        .HasConstraintName("tblSFP${BAED6A63-58FC-494F-9438-DD42268ACE10}")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSystem.Models.TblMn", b =>
                {
                    b.HasOne("DataSystem.Models.TlkpMn", "Mn")
                        .WithMany("TblMn")
                        .HasForeignKey("Mnid");

                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany("TblMn")
                        .HasForeignKey("Nmrid")
                        .HasConstraintName("FK_tblMN_NutritionMonthlyReport")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSystem.Models.TblOtp", b =>
                {
                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany("TblOtp")
                        .HasForeignKey("Nmrid")
                        .HasConstraintName("tblOTP${B1E91814-65DA-4F6F-9B61-FF55C75C6E44}")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.TlkpOtptfu", "Otp")
                        .WithMany("TblOtp")
                        .HasForeignKey("Otpid");
                });

            modelBuilder.Entity("DataSystem.Models.TblOtptfu", b =>
                {
                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany("TblOtptfu")
                        .HasForeignKey("Nmrid")
                        .HasConstraintName("tblOTPTFU${DFECED04-52F4-4144-96A7-63D61F5F34B5}")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.TlkpOtptfu", "Otptfu")
                        .WithMany("TblOtptfu")
                        .HasForeignKey("Otptfuid");
                });

            modelBuilder.Entity("DataSystem.Models.TblQnr", b =>
                {
                    b.HasOne("DataSystem.Models.Implementers", "ImpNavigation")
                        .WithMany()
                        .HasForeignKey("Implementer")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.Provinces", "ProvNavigation")
                        .WithMany()
                        .HasForeignKey("Province");

                    b.HasOne("DataSystem.Models.TblkpStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId");
                });

            modelBuilder.Entity("DataSystem.Models.TblStockIpt", b =>
                {
                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany()
                        .HasForeignKey("Nmrid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.TlkpSstock", "Sstock")
                        .WithMany("TblStockIpt")
                        .HasForeignKey("SstockId");
                });

            modelBuilder.Entity("DataSystem.Models.TblStockOtp", b =>
                {
                    b.HasOne("DataSystem.Models.Nmr", "Nmr")
                        .WithMany()
                        .HasForeignKey("Nmrid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSystem.Models.TlkpSstock", "Sstockotp")
                        .WithMany("TblStockOtp")
                        .HasForeignKey("SstockotpId");
                });
        }
    }
}
