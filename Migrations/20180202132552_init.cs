using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataSystem.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "checkcompleteness",
                columns: table => new
                {
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    District = table.Column<string>(maxLength: 50, nullable: true),
                    FacilityID = table.Column<int>(nullable: false),
                    FacilityName = table.Column<string>(nullable: true),
                    IPDSAM_stock_submission = table.Column<int>(nullable: false),
                    IPDSAM_submission = table.Column<int>(nullable: false),
                    IYCF_submission = table.Column<int>(nullable: false),
                    Implementer = table.Column<string>(maxLength: 20, nullable: true),
                    MNS_submission = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    OPDMAM_stock_submission = table.Column<int>(nullable: false),
                    OPDMAM_submission = table.Column<int>(nullable: false),
                    OPDSAM_stock_submission = table.Column<int>(nullable: false),
                    OPDSAM_submission = table.Column<int>(nullable: false),
                    Province = table.Column<string>(maxLength: 50, nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    message = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkcompleteness", x => x.NMRID);
                });

            migrationBuilder.CreateTable(
                name: "FacilityTypes",
                columns: table => new
                {
                    FacTypeCode = table.Column<int>(nullable: false),
                    FacType = table.Column<string>(maxLength: 50, nullable: true),
                    FacTypeCatCode = table.Column<int>(nullable: true),
                    FacTypeDari = table.Column<string>(maxLength: 50, nullable: true),
                    FacTypePashto = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("FacilityTypes$PrimaryKey", x => x.FacTypeCode);
                });

            migrationBuilder.CreateTable(
                name: "Implementers",
                columns: table => new
                {
                    ImpCode = table.Column<int>(nullable: false),
                    AfghanistanAddress = table.Column<string>(nullable: true),
                    ImpAcronym = table.Column<string>(maxLength: 20, nullable: false),
                    ImpName = table.Column<string>(maxLength: 255, nullable: true),
                    ImpNameDari = table.Column<string>(maxLength: 255, nullable: true),
                    ImpNamePashto = table.Column<string>(maxLength: 255, nullable: true),
                    impStatus = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    OtherAddress = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Implementers$PrimaryKey", x => x.ImpCode);
                });

            migrationBuilder.CreateTable(
                name: "lkpHFStatus",
                columns: table => new
                {
                    HFActiveStatusID = table.Column<int>(nullable: false),
                    HFStatusDescription = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lkpHFStatus", x => x.HFActiveStatusID);
                });

            migrationBuilder.CreateTable(
                name: "MAMReq",
                columns: table => new
                {
                    RID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bhc = table.Column<short>(nullable: false),
                    CHC = table.Column<short>(nullable: false),
                    DH = table.Column<short>(nullable: false),
                    ImpCode = table.Column<string>(nullable: true),
                    MHT = table.Column<short>(nullable: false),
                    Month = table.Column<short>(nullable: false),
                    PH = table.Column<short>(nullable: false),
                    ProvCode = table.Column<string>(maxLength: 50, nullable: false),
                    ReqBy = table.Column<string>(maxLength: 50, nullable: true),
                    ReqMonth = table.Column<short>(nullable: true),
                    ReqYear = table.Column<int>(nullable: true),
                    SHC = table.Column<short>(nullable: false),
                    Tenant = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "date", nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAMReq", x => x.RID);
                });

            migrationBuilder.CreateTable(
                name: "monthlysubmission",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 24, nullable: false),
                    District = table.Column<string>(maxLength: 20, nullable: true),
                    FacilityID = table.Column<int>(nullable: false),
                    FacilityName = table.Column<string>(nullable: true),
                    M1 = table.Column<int>(nullable: false),
                    M10 = table.Column<int>(nullable: false),
                    M11 = table.Column<int>(nullable: false),
                    M12 = table.Column<int>(nullable: false),
                    M2 = table.Column<int>(nullable: false),
                    M3 = table.Column<int>(nullable: false),
                    M4 = table.Column<int>(nullable: false),
                    M5 = table.Column<int>(nullable: false),
                    M6 = table.Column<int>(nullable: false),
                    M7 = table.Column<int>(nullable: false),
                    M8 = table.Column<int>(nullable: false),
                    M9 = table.Column<int>(nullable: false),
                    Province = table.Column<string>(maxLength: 50, nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monthlysubmission", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ProvCode = table.Column<string>(maxLength: 50, nullable: false),
                    AGHCHOCode = table.Column<int>(nullable: true),
                    AIMSCode = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    ProvName = table.Column<string>(maxLength: 255, nullable: false),
                    ProveNameDari = table.Column<string>(maxLength: 50, nullable: false),
                    ProveNamePashto = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Provinces$PrimaryKey", x => x.ProvCode);
                });

            migrationBuilder.CreateTable(
                name: "SAMReq",
                columns: table => new
                {
                    RID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bhc = table.Column<short>(nullable: false),
                    CHC = table.Column<short>(nullable: false),
                    DH = table.Column<short>(nullable: false),
                    ImpCode = table.Column<string>(nullable: false),
                    MHT = table.Column<short>(nullable: false),
                    Month = table.Column<short>(nullable: false),
                    MonthFrom = table.Column<short>(nullable: true),
                    MonthTo = table.Column<short>(nullable: true),
                    PH = table.Column<short>(nullable: false),
                    ProvCode = table.Column<string>(maxLength: 50, nullable: false),
                    ReqBy = table.Column<string>(maxLength: 50, nullable: true),
                    SHC = table.Column<short>(nullable: false),
                    Tenant = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "date", nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Year = table.Column<int>(nullable: false),
                    YearFrom = table.Column<short>(nullable: true),
                    YearTo = table.Column<short>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAMReq", x => x.RID);
                });

            migrationBuilder.CreateTable(
                name: "tblkpStatus",
                columns: table => new
                {
                    StatusID = table.Column<int>(nullable: false),
                    StatusDescription = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblkpStatus", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tlkpForms",
                columns: table => new
                {
                    FormID = table.Column<int>(nullable: false),
                    FormName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkpForms", x => x.FormID);
                });

            migrationBuilder.CreateTable(
                name: "tlkpFstock",
                columns: table => new
                {
                    stockID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    Buffer = table.Column<float>(nullable: false),
                    DistAmountKg = table.Column<decimal>(type: "decimal", nullable: true),
                    Item = table.Column<string>(maxLength: 255, nullable: true),
                    Zarib = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tlkpFstock$stockID", x => x.stockID);
                });

            migrationBuilder.CreateTable(
                name: "tlkpIYCF",
                columns: table => new
                {
                    IYCFID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    cause_consultation = table.Column<string>(maxLength: 255, nullable: true),
                    CauseShortName = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tlkpIYCF$SFPID", x => x.IYCFID);
                });

            migrationBuilder.CreateTable(
                name: "tlkpMN",
                columns: table => new
                {
                    MNID = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    MNitems = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tlkpMN$PrimaryKey", x => x.MNID);
                });

            migrationBuilder.CreateTable(
                name: "tlkpOTPTFU",
                columns: table => new
                {
                    OTPTFUID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    AgeGroup = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tlkpOTPTFU$SFPID", x => x.OTPTFUID);
                });

            migrationBuilder.CreateTable(
                name: "tlkpSFP",
                columns: table => new
                {
                    SFPID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    AgeGroup = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tlkpSFP$SFPID", x => x.SFPID);
                });

            migrationBuilder.CreateTable(
                name: "tlkpSstock",
                columns: table => new
                {
                    sstockID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    Buffer = table.Column<float>(nullable: false),
                    IPDSAMZarib = table.Column<float>(nullable: false),
                    Item = table.Column<string>(maxLength: 255, nullable: true),
                    OPDSAMZarib = table.Column<float>(nullable: false),
                    Persachet = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tlkpSstock$stockID", x => x.sstockID);
                });

            migrationBuilder.CreateTable(
                name: "submissionRes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExDistricts = table.Column<int>(nullable: false),
                    ExFaciliteis = table.Column<int>(nullable: false),
                    ExProvinces = table.Column<int>(nullable: false),
                    SubDistricts = table.Column<int>(nullable: false),
                    SubFacilities = table.Column<int>(nullable: false),
                    SubOrgs = table.Column<int>(nullable: false),
                    SubProvinces = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submissionRes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "nmrsubmission",
                columns: table => new
                {
                    NMRID = table.Column<string>(nullable: false),
                    District = table.Column<string>(maxLength: 50, nullable: true),
                    FacilityID = table.Column<int>(nullable: false),
                    FacilityName = table.Column<string>(nullable: true),
                    FacilityType = table.Column<string>(nullable: true),
                    IPDSAM_stock_submission = table.Column<int>(nullable: false),
                    IPDSAM_submission = table.Column<int>(nullable: false),
                    IYCF_submission = table.Column<int>(nullable: false),
                    Implementer = table.Column<string>(maxLength: 20, nullable: true),
                    MNS_submission = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    OPDMAM_stock_submission = table.Column<int>(nullable: false),
                    OPDMAM_submission = table.Column<int>(nullable: false),
                    OPDSAM_stock_submission = table.Column<int>(nullable: false),
                    OPDSAM_submission = table.Column<int>(nullable: false),
                    ProvCode = table.Column<string>(maxLength: 2, nullable: true),
                    Province = table.Column<string>(maxLength: 50, nullable: true),
                    Year = table.Column<int>(nullable: false),
                    mMonth = table.Column<int>(nullable: false),
                    mYear = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nmrsubmission", x => x.NMRID);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    DistCode = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    DistName = table.Column<string>(maxLength: 255, nullable: false),
                    DistNameDari = table.Column<string>(maxLength: 50, nullable: true),
                    DistNamePashto = table.Column<string>(maxLength: 255, nullable: true),
                    ProvCode = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Districts$PrimaryKey", x => x.DistCode);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvCode",
                        column: x => x.ProvCode,
                        principalTable: "Provinces",
                        principalColumn: "ProvCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblQNR",
                columns: table => new
                {
                    QNRID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Highlights = table.Column<string>(maxLength: 500, nullable: true),
                    Implementer = table.Column<int>(maxLength: 50, nullable: false),
                    IPDSAM_AdmissionsTrend = table.Column<string>(maxLength: 500, nullable: true),
                    IPDSAM_PerformanceTrend = table.Column<string>(maxLength: 500, nullable: true),
                    IYCF = table.Column<string>(maxLength: 500, nullable: true),
                    Micronutrients = table.Column<string>(maxLength: 500, nullable: true),
                    OPDMAM_AdmissionsTrend = table.Column<string>(maxLength: 500, nullable: true),
                    OPDMAM_PerformanceTrend = table.Column<string>(maxLength: 500, nullable: true),
                    OPDSAM_AdmissionsTrend = table.Column<string>(maxLength: 500, nullable: true),
                    OPDSAM_PerformanceTrend = table.Column<string>(maxLength: 500, nullable: true),
                    Province = table.Column<string>(maxLength: 50, nullable: true),
                    ReportMonth = table.Column<int>(nullable: true),
                    ReportYear = table.Column<int>(nullable: true),
                    ReportingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    Tenant = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblQNR", x => x.QNRID);
                    table.ForeignKey(
                        name: "FK_tblQNR_Implementers_Implementer",
                        column: x => x.Implementer,
                        principalTable: "Implementers",
                        principalColumn: "ImpCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblQNR_Provinces_Province",
                        column: x => x.Province,
                        principalTable: "Provinces",
                        principalColumn: "ProvCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblQNR_tblkpStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "tblkpStatus",
                        principalColumn: "StatusID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MAMReqDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Adjustment = table.Column<int>(nullable: true),
                    AdjustmentComment = table.Column<string>(maxLength: 255, nullable: true),
                    CurrentBalance = table.Column<int>(nullable: true),
                    FormName = table.Column<string>(nullable: true),
                    NoOfBenificiaries = table.Column<int>(nullable: true),
                    RID = table.Column<long>(nullable: false),
                    SupplyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAMReqDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MAMReqDetails_MAMReq",
                        column: x => x.RID,
                        principalTable: "MAMReq",
                        principalColumn: "RID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mam_supply",
                        column: x => x.SupplyId,
                        principalTable: "tlkpFstock",
                        principalColumn: "stockID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SAMReqDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Adjustment = table.Column<int>(nullable: true),
                    AdjustmentComment = table.Column<string>(maxLength: 255, nullable: true),
                    CurrentBalance = table.Column<int>(nullable: true),
                    FormName = table.Column<string>(maxLength: 50, nullable: false),
                    O6 = table.Column<int>(nullable: true),
                    RID = table.Column<long>(nullable: false),
                    SupplyId = table.Column<int>(nullable: false),
                    U6 = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "date", nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAMReqDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SAMReqDetails_SAMReq",
                        column: x => x.RID,
                        principalTable: "SAMReq",
                        principalColumn: "RID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblSstock_tlkpSstock",
                        column: x => x.SupplyId,
                        principalTable: "tlkpSstock",
                        principalColumn: "sstockID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacilityInfo",
                columns: table => new
                {
                    FacilityID = table.Column<int>(nullable: false),
                    ActiveStatus = table.Column<string>(maxLength: 10, nullable: true),
                    DateEstablished = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    DistCode = table.Column<string>(maxLength: 50, nullable: false),
                    FacilityName = table.Column<string>(maxLength: 255, nullable: false),
                    FacilityNameDari = table.Column<string>(maxLength: 255, nullable: true),
                    FacilityNamePashto = table.Column<string>(maxLength: 255, nullable: true),
                    FacilityType = table.Column<int>(nullable: false),
                    GPSLattitude = table.Column<double>(nullable: true),
                    GPSLongtitude = table.Column<double>(nullable: true),
                    Implementer = table.Column<string>(nullable: false),
                    LAT = table.Column<double>(nullable: true),
                    Location = table.Column<string>(maxLength: 100, nullable: true),
                    LocationDari = table.Column<string>(maxLength: 100, nullable: true),
                    LocationPashto = table.Column<string>(maxLength: 100, nullable: true),
                    LON = table.Column<double>(nullable: true),
                    SubImplementer = table.Column<string>(maxLength: 255, nullable: true),
                    ViliCode = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("FacilityInfo$PrimaryKey", x => x.FacilityID);
                    table.ForeignKey(
                        name: "FK_FacilityInfo_Districts_DistCode",
                        column: x => x.DistCode,
                        principalTable: "Districts",
                        principalColumn: "DistCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacilityInfo_FacilityTypes_FacilityType",
                        column: x => x.FacilityType,
                        principalTable: "FacilityTypes",
                        principalColumn: "FacTypeCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NMR",
                columns: table => new
                {
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    BoysScreened = table.Column<int>(nullable: true),
                    commen = table.Column<string>(nullable: true),
                    FacilityID = table.Column<int>(nullable: false),
                    FacilityType = table.Column<int>(nullable: true),
                    FLANumber = table.Column<int>(nullable: true),
                    GirlsScreened = table.Column<int>(nullable: true),
                    HFActiveStatusID = table.Column<int>(nullable: true),
                    IALS_Kwashiorkor = table.Column<int>(nullable: true),
                    IALS_Marasmus = table.Column<int>(nullable: true),
                    IAWG_Kwashiorkor = table.Column<int>(nullable: true),
                    IAWG_Marasmus = table.Column<int>(nullable: true),
                    Implementer = table.Column<string>(maxLength: 255, nullable: false),
                    IpdAdmissionsByChws = table.Column<int>(nullable: true),
                    IpdRutfstockOutWeeks = table.Column<int>(nullable: true),
                    MamAddminsionByChws = table.Column<int>(nullable: true),
                    MamRusfstockoutWeeks = table.Column<int>(nullable: true),
                    Month = table.Column<int>(nullable: false),
                    OALS_Kwashiorkor = table.Column<int>(nullable: true),
                    OALS_Marasmus = table.Column<int>(nullable: true),
                    OAWG_Kwashiorkor = table.Column<int>(nullable: true),
                    OAWG_Marasmus = table.Column<int>(nullable: true),
                    OpdAdmissionsByChws = table.Column<int>(nullable: true),
                    OpdRutfstockOutWeeks = table.Column<int>(nullable: true),
                    OpeningDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Plwreported = table.Column<int>(nullable: true),
                    PreparedBy = table.Column<string>(maxLength: 255, nullable: true),
                    SFP_ALS = table.Column<int>(nullable: true),
                    SFP_AWG = table.Column<int>(nullable: true),
                    StatusID = table.Column<int>(nullable: true),
                    Tenant = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    Year = table.Column<int>(nullable: false),
                    isHumanitarian = table.Column<bool>(nullable: false),
                    mMonth = table.Column<int>(nullable: false),
                    mYear = table.Column<int>(nullable: false),
                    message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NMR", x => x.NMRID);
                    table.ForeignKey(
                        name: "FK_NutritionMonthlyReport_FacilityInfo",
                        column: x => x.FacilityID,
                        principalTable: "FacilityInfo",
                        principalColumn: "FacilityID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NMR_lkpHFStatus",
                        column: x => x.HFActiveStatusID,
                        principalTable: "lkpHFStatus",
                        principalColumn: "HFActiveStatusID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NMR_tblkpStatus_StatusID",
                        column: x => x.StatusID,
                        principalTable: "tblkpStatus",
                        principalColumn: "StatusID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblFeedback",
                columns: table => new
                {
                    FedID = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    FormID = table.Column<int>(nullable: true),
                    NMRID = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFeedback", x => x.FedID);
                    table.ForeignKey(
                        name: "FK_tblFeedback_tlkpForms_FormID",
                        column: x => x.FormID,
                        principalTable: "tlkpForms",
                        principalColumn: "FormID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblFeedback_NMR",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblFstock",
                columns: table => new
                {
                    stockID = table.Column<int>(nullable: false),
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    ExpectedRecepients = table.Column<int>(nullable: true),
                    Losses = table.Column<int>(nullable: true),
                    OpeningBalance = table.Column<int>(nullable: true),
                    QuantityDistributed = table.Column<int>(nullable: true),
                    QuantityReceived = table.Column<int>(nullable: true),
                    QuantityReferin = table.Column<int>(nullable: true),
                    QuantityReturned = table.Column<int>(nullable: true),
                    QuantityTransferred = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tblFstock$PrimaryKey", x => new { x.stockID, x.NMRID });
                    table.ForeignKey(
                        name: "FK_tblFstock_NMR_NMRID",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFstock_tlkpFstock_stockID",
                        column: x => x.stockID,
                        principalTable: "tlkpFstock",
                        principalColumn: "stockID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblIYCF",
                columns: table => new
                {
                    IYCFID = table.Column<int>(nullable: false),
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    Firstvisit = table.Column<int>(nullable: true),
                    mChild524months = table.Column<int>(nullable: true),
                    mChildU5months = table.Column<int>(nullable: true),
                    pregnanatwomen = table.Column<int>(nullable: true),
                    ReferIn = table.Column<int>(nullable: true),
                    ReferOut = table.Column<int>(nullable: true),
                    Revisit = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tblIYCF$PrimaryKey", x => new { x.IYCFID, x.NMRID });
                    table.ForeignKey(
                        name: "FK_tblIYCF_tlkpIYCF_IYCFID",
                        column: x => x.IYCFID,
                        principalTable: "tlkpIYCF",
                        principalColumn: "IYCFID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "tblIYCF${AC5A7038-0E9D-4C9A-A138-0D220C0EE76E}",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblMAM",
                columns: table => new
                {
                    MAMID = table.Column<int>(nullable: false),
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    Absents = table.Column<int>(nullable: true),
                    Cured = table.Column<int>(nullable: true),
                    Deaths = table.Column<int>(nullable: true),
                    Defaulters = table.Column<int>(nullable: true),
                    MUAC12 = table.Column<int>(nullable: true),
                    MUAC23 = table.Column<int>(nullable: true),
                    NonCured = table.Column<int>(nullable: true),
                    ReferIn = table.Column<int>(nullable: true),
                    tFemale = table.Column<int>(nullable: true),
                    tMale = table.Column<int>(nullable: true),
                    totalbegin = table.Column<int>(nullable: true),
                    Transfers = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    Zscore23 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tblSFP$PrimaryKey", x => new { x.MAMID, x.NMRID });
                    table.ForeignKey(
                        name: "FK_tblMAM_tlkpSFP_MAMID",
                        column: x => x.MAMID,
                        principalTable: "tlkpSFP",
                        principalColumn: "SFPID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "tblSFP${BAED6A63-58FC-494F-9438-DD42268ACE10}",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblMN",
                columns: table => new
                {
                    MNID = table.Column<int>(nullable: false),
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    chu2f = table.Column<int>(nullable: true),
                    chu2m = table.Column<int>(nullable: true),
                    refbyChw = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tblMN$PrimaryKey", x => new { x.MNID, x.NMRID });
                    table.ForeignKey(
                        name: "FK_tblMN_tlkpMN_MNID",
                        column: x => x.MNID,
                        principalTable: "tlkpMN",
                        principalColumn: "MNID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblMN_NutritionMonthlyReport",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblOTP",
                columns: table => new
                {
                    OTPID = table.Column<int>(nullable: false),
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    Cured = table.Column<int>(nullable: true),
                    Death = table.Column<int>(nullable: true),
                    Default = table.Column<int>(nullable: true),
                    defaultreturn = table.Column<int>(nullable: true),
                    fromscotp = table.Column<int>(nullable: true),
                    fromsfp = table.Column<int>(nullable: true),
                    MUAC115 = table.Column<int>(nullable: true),
                    NonCured = table.Column<int>(nullable: true),
                    odema = table.Column<int>(nullable: true),
                    RefOut = table.Column<int>(nullable: true),
                    tFemale = table.Column<int>(nullable: true),
                    tMale = table.Column<int>(nullable: true),
                    totalbegin = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    z3score = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tblOTP$PrimaryKey", x => new { x.OTPID, x.NMRID });
                    table.ForeignKey(
                        name: "tblOTP${B1E91814-65DA-4F6F-9B61-FF55C75C6E44}",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblOTP_tlkpOTPTFU_OTPID",
                        column: x => x.OTPID,
                        principalTable: "tlkpOTPTFU",
                        principalColumn: "OTPTFUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblOTPTFU",
                columns: table => new
                {
                    OTPTFUID = table.Column<int>(nullable: false),
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    Cured = table.Column<int>(nullable: true),
                    Death = table.Column<int>(nullable: true),
                    Default = table.Column<int>(nullable: true),
                    defaultreturn = table.Column<int>(nullable: true),
                    fromscotp = table.Column<int>(nullable: true),
                    fromsfp = table.Column<int>(nullable: true),
                    MUAC115 = table.Column<int>(nullable: true),
                    NonCured = table.Column<int>(nullable: true),
                    odema = table.Column<int>(nullable: true),
                    RefOut = table.Column<int>(nullable: true),
                    tFemale = table.Column<int>(nullable: true),
                    tMale = table.Column<int>(nullable: true),
                    totalbegin = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    z3score = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tblOTPTFU$PrimaryKey", x => new { x.OTPTFUID, x.NMRID });
                    table.ForeignKey(
                        name: "tblOTPTFU${DFECED04-52F4-4144-96A7-63D61F5F34B5}",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblOTPTFU_tlkpOTPTFU_OTPTFUID",
                        column: x => x.OTPTFUID,
                        principalTable: "tlkpOTPTFU",
                        principalColumn: "OTPTFUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblStock_ipt",
                columns: table => new
                {
                    sstockID = table.Column<int>(nullable: false),
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    Damaged = table.Column<decimal>(type: "decimal", nullable: true),
                    Expired = table.Column<decimal>(type: "decimal", nullable: true),
                    Loss = table.Column<decimal>(type: "decimal", nullable: true),
                    Openingbalance = table.Column<decimal>(type: "decimal", nullable: true),
                    Received = table.Column<decimal>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Used = table.Column<decimal>(type: "decimal", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tblSstock$PrimaryKey", x => new { x.sstockID, x.NMRID });
                    table.ForeignKey(
                        name: "FK_tblStock_ipt_NMR_NMRID",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblStock_ipt_tlkpSstock_sstockID",
                        column: x => x.sstockID,
                        principalTable: "tlkpSstock",
                        principalColumn: "sstockID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblStock_otp",
                columns: table => new
                {
                    sstockotpID = table.Column<int>(nullable: false),
                    NMRID = table.Column<string>(maxLength: 100, nullable: false),
                    Damaged = table.Column<decimal>(nullable: true),
                    Expired = table.Column<decimal>(nullable: true),
                    Loss = table.Column<decimal>(type: "decimal", nullable: true),
                    Openingbalance = table.Column<decimal>(type: "decimal", nullable: true),
                    Received = table.Column<decimal>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Used = table.Column<decimal>(nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tblSstock_otp$PrimaryKey", x => new { x.sstockotpID, x.NMRID });
                    table.ForeignKey(
                        name: "FK_tblStock_otp_NMR_NMRID",
                        column: x => x.NMRID,
                        principalTable: "NMR",
                        principalColumn: "NMRID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblStock_otp_tlkpSstock_sstockotpID",
                        column: x => x.sstockotpID,
                        principalTable: "tlkpSstock",
                        principalColumn: "sstockID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "Districts$DistName",
                table: "Districts",
                column: "DistName");

            migrationBuilder.CreateIndex(
                name: "Districts$DistNameDari",
                table: "Districts",
                column: "DistNameDari");

            migrationBuilder.CreateIndex(
                name: "Districts$PROV_34_ID",
                table: "Districts",
                column: "ProvCode");

            migrationBuilder.CreateIndex(
                name: "FacilityInfo$DistCode",
                table: "FacilityInfo",
                column: "DistCode");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityInfo_FacilityType",
                table: "FacilityInfo",
                column: "FacilityType");

            migrationBuilder.CreateIndex(
                name: "FacilityInfo$GOCode",
                table: "FacilityInfo",
                column: "ViliCode");

            migrationBuilder.CreateIndex(
                name: "FacilityTypes$FacTypeCatCode",
                table: "FacilityTypes",
                column: "FacTypeCatCode");

            migrationBuilder.CreateIndex(
                name: "Implementers$UNOPSCode",
                table: "Implementers",
                column: "ImpAcronym");

            migrationBuilder.CreateIndex(
                name: "Implementers$MRRD_Code",
                table: "Implementers",
                column: "ImpName");

            migrationBuilder.CreateIndex(
                name: "UQ__MAMReq__98EDC49B9F16E08E",
                table: "MAMReq",
                columns: new[] { "ProvCode", "ImpCode", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MAMReqDetails_SupplyId",
                table: "MAMReqDetails",
                column: "SupplyId");

            migrationBuilder.CreateIndex(
                name: "UQ__MAMReqDe__3D3297F9B5B94ECB",
                table: "MAMReqDetails",
                columns: new[] { "RID", "SupplyId", "FormName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "NutritionMonthlyReport$FacilityInfoNutritionMonthlyReport",
                table: "NMR",
                column: "FacilityID");

            migrationBuilder.CreateIndex(
                name: "IX_NMR_HFActiveStatusID",
                table: "NMR",
                column: "HFActiveStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_NMR_StatusID",
                table: "NMR",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "Provinces$ProvName",
                table: "Provinces",
                column: "ProvName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Provinces$ProveNameDari",
                table: "Provinces",
                column: "ProveNameDari",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Provinces$ProveNamePashtu",
                table: "Provinces",
                column: "ProveNamePashto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SAMReq__E61DE2A759B61604",
                table: "SAMReq",
                columns: new[] { "ProvCode", "ImpCode", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SAMReqDetails_SupplyId",
                table: "SAMReqDetails",
                column: "SupplyId");

            migrationBuilder.CreateIndex(
                name: "UQ__SAMReqDe__12B320735C79634D",
                table: "SAMReqDetails",
                columns: new[] { "RID", "SupplyId", "FormName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblFeedback_FormID",
                table: "tblFeedback",
                column: "FormID");

            migrationBuilder.CreateIndex(
                name: "IX_tblFeedback_NMRID",
                table: "tblFeedback",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblFstock$NMRID",
                table: "tblFstock",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblFstock$stockID1",
                table: "tblFstock",
                column: "stockID");

            migrationBuilder.CreateIndex(
                name: "tblIYCF$InputID",
                table: "tblIYCF",
                column: "IYCFID");

            migrationBuilder.CreateIndex(
                name: "tblIYCF${AC5A7038-0E9D-4C9A-A138-0D220C0EE76E}",
                table: "tblIYCF",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblSFP$SFPID",
                table: "tblMAM",
                column: "MAMID");

            migrationBuilder.CreateIndex(
                name: "tblSFP$NMRID",
                table: "tblMAM",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblMN${58F5E5B9-3552-4441-8E08-55B12448DD25}",
                table: "tblMN",
                column: "MNID");

            migrationBuilder.CreateIndex(
                name: "tblMN$NMRID",
                table: "tblMN",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblOTP$NMRID",
                table: "tblOTP",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblOTP$SFPID",
                table: "tblOTP",
                column: "OTPID");

            migrationBuilder.CreateIndex(
                name: "tblOTPTFU$NMRID",
                table: "tblOTPTFU",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblOTPTFU$SFPID",
                table: "tblOTPTFU",
                column: "OTPTFUID");

            migrationBuilder.CreateIndex(
                name: "IX_tblQNR_Implementer",
                table: "tblQNR",
                column: "Implementer");

            migrationBuilder.CreateIndex(
                name: "IX_tblQNR_Province",
                table: "tblQNR",
                column: "Province");

            migrationBuilder.CreateIndex(
                name: "IX_tblQNR_StatusId",
                table: "tblQNR",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "tblSstock$NMRID",
                table: "tblStock_ipt",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblSstock$stockID1",
                table: "tblStock_ipt",
                column: "sstockID");

            migrationBuilder.CreateIndex(
                name: "tblSstock_otp$NMRID",
                table: "tblStock_otp",
                column: "NMRID");

            migrationBuilder.CreateIndex(
                name: "tblSstock_otp$stockID1",
                table: "tblStock_otp",
                column: "sstockotpID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkcompleteness");

            migrationBuilder.DropTable(
                name: "MAMReqDetails");

            migrationBuilder.DropTable(
                name: "monthlysubmission");

            migrationBuilder.DropTable(
                name: "SAMReqDetails");

            migrationBuilder.DropTable(
                name: "tblFeedback");

            migrationBuilder.DropTable(
                name: "tblFstock");

            migrationBuilder.DropTable(
                name: "tblIYCF");

            migrationBuilder.DropTable(
                name: "tblMAM");

            migrationBuilder.DropTable(
                name: "tblMN");

            migrationBuilder.DropTable(
                name: "tblOTP");

            migrationBuilder.DropTable(
                name: "tblOTPTFU");

            migrationBuilder.DropTable(
                name: "tblQNR");

            migrationBuilder.DropTable(
                name: "tblStock_ipt");

            migrationBuilder.DropTable(
                name: "tblStock_otp");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "submissionRes");

            migrationBuilder.DropTable(
                name: "nmrsubmission");

            migrationBuilder.DropTable(
                name: "MAMReq");

            migrationBuilder.DropTable(
                name: "SAMReq");

            migrationBuilder.DropTable(
                name: "tlkpForms");

            migrationBuilder.DropTable(
                name: "tlkpFstock");

            migrationBuilder.DropTable(
                name: "tlkpIYCF");

            migrationBuilder.DropTable(
                name: "tlkpSFP");

            migrationBuilder.DropTable(
                name: "tlkpMN");

            migrationBuilder.DropTable(
                name: "tlkpOTPTFU");

            migrationBuilder.DropTable(
                name: "Implementers");

            migrationBuilder.DropTable(
                name: "NMR");

            migrationBuilder.DropTable(
                name: "tlkpSstock");

            migrationBuilder.DropTable(
                name: "FacilityInfo");

            migrationBuilder.DropTable(
                name: "lkpHFStatus");

            migrationBuilder.DropTable(
                name: "tblkpStatus");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "FacilityTypes");

            migrationBuilder.DropTable(
                name: "Provinces");
        }
    }
}
