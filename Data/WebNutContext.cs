using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DataSystem.Models.ViewModels.chart;
using DataSystem.Models.ViewModels.Export;
using DataSystem.Models.ViewModels.PivotTable;
using DataSystem.Models.ViewModels;
using DataSystem.Models.Checklist_subs;
using DataSystem.Models.SCM;
using DataSystem.Models.HP;
using DataSystem.GLM.Models;
using DataSystem.Controllers.SCM;
using DataSystem.Models.GLM;


namespace DataSystem.Models
{
    public partial class WebNutContext : DbContext
    {
        public WebNutContext(DbContextOptions<WebNutContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<checkcompleteness>(entity =>
{
    entity.Property(e => e.NMRID).HasMaxLength(100);
    entity.Property(e => e.Province).HasMaxLength(50);
    entity.Property(e => e.District).HasMaxLength(50);
    entity.Property(e => e.FacilityID);
    entity.Property(e => e.FacilityName);
    entity.Property(e => e.Implementer).HasMaxLength(20);
    entity.Property(e => e.message).HasMaxLength(500);
    entity.Property(e => e.StatusId);
    entity.Property(e => e.Year);
    entity.Property(e => e.Month);
    entity.Property(e => e.IPDSAM_submission);
    entity.Property(e => e.OPDSAM_submission);
    entity.Property(e => e.OPDMAM_submission);
    entity.Property(e => e.MNS_submission);
    entity.Property(e => e.OPDMAM_stock_submission);
    entity.Property(e => e.IPDSAM_stock_submission);
    entity.Property(e => e.OPDSAM_stock_submission);
    entity.Property(e => e.UserName);
    entity.Property(e => e.Tenant);
    entity.Property(e => e.MyId);
});

            modelBuilder.Entity<submissionRes>(entity =>
            {
                entity.Property(e => e.ID);
                entity.Property(e => e.SubProvinces);
                entity.Property(e => e.SubDistricts);
                entity.Property(e => e.SubFacilities);
                entity.Property(e => e.SubOrgs);
            });

            modelBuilder.Entity<monthlysubmission>(entity =>
            {
                entity.Property(e => e.ID).HasMaxLength(24);
                entity.Property(e => e.Province).HasMaxLength(50);
                entity.Property(e => e.District).HasMaxLength(20);
                entity.Property(e => e.FacilityID);
                entity.Property(e => e.FacilityName);
                entity.Property(e => e.UserName);
                entity.Property(e => e.Year);
                entity.Property(e => e.M1);
                entity.Property(e => e.M2);
                entity.Property(e => e.M3);
                entity.Property(e => e.M4);
                entity.Property(e => e.M5);
                entity.Property(e => e.M6);
                entity.Property(e => e.M7);
                entity.Property(e => e.M8);
                entity.Property(e => e.M9);
                entity.Property(e => e.M10);
                entity.Property(e => e.M11);
                entity.Property(e => e.M12);
                entity.Property(e => e.ProvId);
            });

            modelBuilder.Entity<nmrsubmission>(entity =>
            {
                entity.Property(e => e.District).HasMaxLength(50);
                entity.Property(e => e.Implementer).HasMaxLength(20);
                entity.Property(e => e.FacilityID);
                entity.Property(e => e.FacilityName);
                entity.Property(e => e.FacilityType);
                entity.Property(e => e.ProvCode).HasMaxLength(2);
                entity.Property(e => e.Province).HasMaxLength(50);
                entity.Property(e => e.Year);
                entity.Property(e => e.Month);
                entity.Property(e => e.mYear);
                entity.Property(e => e.mMonth);
                entity.Property(e => e.IPDSAM_submission);
                entity.Property(e => e.OPDSAM_submission);
                entity.Property(e => e.OPDMAM_submission);
                entity.Property(e => e.MNS_submission);
                entity.Property(e => e.OPDMAM_stock_submission);
                entity.Property(e => e.IPDSAM_stock_submission);
                entity.Property(e => e.OPDSAM_stock_submission);
            });

            modelBuilder.Entity<Districts>(entity =>
            {
                entity.HasKey(e => e.DistCode)
                    .HasName("Districts$PrimaryKey");

                entity.HasIndex(e => e.DistName)
                    .HasName("Districts$DistName");

                entity.HasIndex(e => e.DistNameDari)
                    .HasName("Districts$DistNameDari");

                entity.HasIndex(e => e.ProvCode)
                    .HasName("Districts$PROV_34_ID");

                entity.Property(e => e.DistCode).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.DistName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DistNameDari).HasMaxLength(50);

                entity.Property(e => e.DistNamePashto).HasMaxLength(255);

                entity.Property(e => e.ProvCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.ProvCodeNavigation)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvCode)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Districts${FD557F29-E1F2-49D5-A2AA-D987E7AF8819}");
            });

            modelBuilder.Entity<FacilityInfo>(entity =>
            {
                entity.HasKey(e => e.FacilityId)
                    .HasName("FacilityInfo$PrimaryKey");

                entity.HasIndex(e => e.DistCode)
                    .HasName("FacilityInfo$DistCode");

                entity.HasIndex(e => e.ViliCode)
                    .HasName("FacilityInfo$GOCode");

                entity.Property(e => e.FacilityId)
                    .HasColumnName("FacilityID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DistCode).HasMaxLength(50);

                entity.Property(e => e.ActiveStatus).HasMaxLength(10);

                entity.Property(e => e.DateEstablished).HasColumnType("datetime2(0)");

                entity.Property(e => e.FacilityName).HasMaxLength(255);

                entity.Property(e => e.FacilityNameDari).HasMaxLength(255);

                entity.Property(e => e.FacilityNamePashto).HasMaxLength(255);

                entity.Property(e => e.Gpslattitude).HasColumnName("GPSLattitude");

                entity.Property(e => e.Gpslongtitude).HasColumnName("GPSLongtitude");

                entity.Property(e => e.Lat).HasColumnName("LAT");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.LocationDari).HasMaxLength(100);

                entity.Property(e => e.LocationPashto).HasMaxLength(100);

                entity.Property(e => e.Lon).HasColumnName("LON");

                entity.Property(e => e.SubImplementer).HasMaxLength(255);

                entity.Property(e => e.ViliCode).HasMaxLength(255);
            });

            modelBuilder.Entity<FacilityTypes>(entity =>
            {
                entity.HasKey(e => e.FacTypeCode)
                    .HasName("FacilityTypes$PrimaryKey");

                entity.HasIndex(e => e.FacTypeCatCode)
                    .HasName("FacilityTypes$FacTypeCatCode");

                entity.Property(e => e.FacTypeCode).ValueGeneratedNever();

                entity.Property(e => e.FacType).HasMaxLength(50);

                entity.Property(e => e.FacTypeDari).HasMaxLength(50);

                entity.Property(e => e.FacTypePashto).HasMaxLength(50);
                entity.Property(e => e.TypeAbbrv).HasMaxLength(10);
            });

            modelBuilder.Entity<Implementers>(entity =>
            {
                entity.HasKey(e => e.ImpCode)
                    .HasName("Implementers$PrimaryKey");

                entity.HasIndex(e => e.ImpAcronym)
                    .HasName("Implementers$UNOPSCode");

                entity.HasIndex(e => e.ImpName)
                    .HasName("Implementers$MRRD_Code");

                entity.Property(e => e.ImpCode).ValueGeneratedNever();

                entity.Property(e => e.ImpAcronym)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ImpName).HasMaxLength(255);

                entity.Property(e => e.ImpNameDari).HasMaxLength(255);

                entity.Property(e => e.ImpNamePashto).HasMaxLength(255);

                entity.Property(e => e.ImpStatus)
                    .HasColumnName("impStatus")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.RegistrationDate).HasColumnType("datetime2(0)");
            });

            modelBuilder.Entity<LkpHfstatus>(entity =>
            {
                entity.HasKey(e => e.HfactiveStatusId)
                    .HasName("PK_lkpHFStatus");

                entity.ToTable("lkpHFStatus");

                entity.Property(e => e.HfactiveStatusId)
                    .HasColumnName("HFActiveStatusID")
                    .ValueGeneratedNever();

                entity.Property(e => e.HfstatusDescription)
                    .HasColumnName("HFStatusDescription")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TblQnr>(entity =>
            {
                entity.HasKey(e => e.Qnrid)
                    .HasName("PK_tblQNR");

                entity.ToTable("tblQNR");

                entity.Property(e => e.Qnrid).HasColumnName("QNRID");

                entity.Property(e => e.Highlights).HasMaxLength(500);

                entity.Property(e => e.Implementer).HasMaxLength(50);

                entity.Property(e => e.IpdsamAdmissionsTrend)
                    .HasColumnName("IPDSAM_AdmissionsTrend")
                    .HasMaxLength(500);

                entity.Property(e => e.IpdsamPerformanceTrend)
                    .HasColumnName("IPDSAM_PerformanceTrend")
                    .HasMaxLength(500);

                entity.Property(e => e.Iycf)
                    .HasColumnName("IYCF")
                    .HasMaxLength(500);

                entity.Property(e => e.Micronutrients).HasMaxLength(500);

                entity.Property(e => e.OpdmamAdmissionsTrend)
                    .HasColumnName("OPDMAM_AdmissionsTrend")
                    .HasMaxLength(500);

                entity.Property(e => e.OpdmamPerformanceTrend)
                    .HasColumnName("OPDMAM_PerformanceTrend")
                    .HasMaxLength(500);

                entity.Property(e => e.OpdsamAdmissionsTrend)
                    .HasColumnName("OPDSAM_AdmissionsTrend")
                    .HasMaxLength(500);

                entity.Property(e => e.OpdsamPerformanceTrend)
                    .HasColumnName("OPDSAM_PerformanceTrend")
                    .HasMaxLength(500);

                entity.Property(e => e.Province).HasMaxLength(50);

                entity.Property(e => e.ReportingDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<Nmr>(entity =>
            {
                entity.ToTable("NMR");

                entity.HasIndex(e => e.FacilityId)
                    .HasName("NutritionMonthlyReport$FacilityInfoNutritionMonthlyReport");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.Commen).HasColumnName("commen");

                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");

                entity.Property(e => e.Flanumber).HasColumnName("FLANumber");

                entity.Property(e => e.HfactiveStatusId).HasColumnName("HFActiveStatusID");

                entity.Property(e => e.IalsKwashiorkor).HasColumnName("IALS_Kwashiorkor");

                entity.Property(e => e.IalsMarasmus).HasColumnName("IALS_Marasmus");

                entity.Property(e => e.IawgKwashiorkor).HasColumnName("IAWG_Kwashiorkor");

                entity.Property(e => e.IawgMarasmus).HasColumnName("IAWG_Marasmus");

                entity.Property(e => e.Implementer)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.OalsKwashiorkor).HasColumnName("OALS_Kwashiorkor");

                entity.Property(e => e.OalsMarasmus).HasColumnName("OALS_Marasmus");

                entity.Property(e => e.OawgKwashiorkor).HasColumnName("OAWG_Kwashiorkor");

                entity.Property(e => e.OawgMarasmus).HasColumnName("OAWG_Marasmus");

                entity.Property(e => e.OpeningDate).HasColumnType("datetime");

                entity.Property(e => e.PreparedBy).HasMaxLength(255);

                entity.Property(e => e.SfpAls).HasColumnName("SFP_ALS");

                entity.Property(e => e.SfpAwg).HasColumnName("SFP_AWG");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Facility)
                    .WithMany(p => p.Nmr)
                    .HasForeignKey(d => d.FacilityId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_NutritionMonthlyReport_FacilityInfo");

                entity.HasOne(d => d.HfactiveStatus)
                    .WithMany(p => p.Nmr)
                    .HasForeignKey(d => d.HfactiveStatusId)
                    .HasConstraintName("FK_NMR_lkpHFStatus");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Nmr)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_NutritionMonthlyReport_tblkpStatus");
            });

            modelBuilder.Entity<Provinces>(entity =>
            {
                entity.HasKey(e => e.ProvCode)
                    .HasName("Provinces$PrimaryKey");

                entity.HasIndex(e => e.ProvName)
                    .HasName("Provinces$ProvName")
                    .IsUnique();

                entity.HasIndex(e => e.ProveNameDari)
                    .HasName("Provinces$ProveNameDari")
                    .IsUnique();

                entity.HasIndex(e => e.ProveNamePashto)
                    .HasName("Provinces$ProveNamePashtu")
                    .IsUnique();

                entity.Property(e => e.ProvCode).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.ProvName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ProveNameDari)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProveNamePashto)
                    .IsRequired()
                    .HasMaxLength(255);
            });



            modelBuilder.Entity<TblFeedback>(entity =>
            {
                entity.HasKey(e => e.FedId)
                    .HasName("PK_tblFeedback");

                entity.ToTable("tblFeedback");

                entity.Property(e => e.FedId)
                    .HasColumnName("FedID")
                    .ValueGeneratedNever();

                entity.Property(e => e.FormId).HasColumnName("FormID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.TblFeedback)
                    .HasForeignKey(d => d.FormId)
                    .HasConstraintName("FK_tblFeedback_tlkpForms");

                entity.HasOne(d => d.Nmr)
                    .WithMany(p => p.TblFeedback)
                    .HasForeignKey(d => d.Nmrid)
                    .HasConstraintName("FK_tblFeedback_NMR");
            });

            modelBuilder.Entity<TblFstock>(entity =>
            {
                entity.HasKey(e => new { e.StockId, e.Nmrid })
                    .HasName("tblFstock$PrimaryKey");

                entity.ToTable("tblFstock");

                entity.HasIndex(e => e.Nmrid)
                    .HasName("tblFstock$NMRID");

                entity.HasIndex(e => e.StockId)
                    .HasName("tblFstock$stockID1");

                entity.Property(e => e.StockId).HasColumnName("stockID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.TblFstock)
                    .HasForeignKey(d => d.StockId)
                    .HasConstraintName("FK_tblFstock_tlkpFstock");
            });

            modelBuilder.Entity<TblIycf>(entity =>
            {
                entity.HasKey(e => new { e.Iycfid, e.Nmrid })
                    .HasName("tblIYCF$PrimaryKey");

                entity.ToTable("tblIYCF");

                entity.HasIndex(e => e.Iycfid)
                    .HasName("tblIYCF$InputID");

                entity.HasIndex(e => e.Nmrid)
                    .HasName("tblIYCF${AC5A7038-0E9D-4C9A-A138-0D220C0EE76E}");

                entity.Property(e => e.Iycfid).HasColumnName("IYCFID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.MChild524months).HasColumnName("mChild524months");

                entity.Property(e => e.MChildU5months).HasColumnName("mChildU5months");

                entity.Property(e => e.Pregnanatwomen).HasColumnName("pregnanatwomen");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Iycf)
                    .WithMany(p => p.TblIycf)
                    .HasForeignKey(d => d.Iycfid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblIYCF_tlkpIYCF");

                entity.HasOne(d => d.Nmr)
                    .WithMany(p => p.TblIycf)
                    .HasForeignKey(d => d.Nmrid)
                    .HasConstraintName("tblIYCF${AC5A7038-0E9D-4C9A-A138-0D220C0EE76E}");
            });

            modelBuilder.Entity<TblMam>(entity =>
            {
                entity.HasKey(e => new { e.Mamid, e.Nmrid })
                    .HasName("tblSFP$PrimaryKey");

                entity.ToTable("tblMAM");

                entity.HasIndex(e => e.Mamid)
                    .HasName("tblSFP$SFPID");

                entity.HasIndex(e => e.Nmrid)
                    .HasName("tblSFP$NMRID");

                entity.Property(e => e.Mamid).HasColumnName("MAMID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.Muac12).HasColumnName("MUAC12");

                entity.Property(e => e.Muac23).HasColumnName("MUAC23");

                entity.Property(e => e.TFemale).HasColumnName("tFemale");

                entity.Property(e => e.TMale).HasColumnName("tMale");

                entity.Property(e => e.Totalbegin).HasColumnName("totalbegin");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Mam)
                    .WithMany(p => p.TblMam)
                    .HasForeignKey(d => d.Mamid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblSFP_tlkpSFP");

                entity.HasOne(d => d.Nmr)
                    .WithMany(p => p.TblMam)
                    .HasForeignKey(d => d.Nmrid)
                    .HasConstraintName("tblSFP${BAED6A63-58FC-494F-9438-DD42268ACE10}");
            });

            modelBuilder.Entity<TblMn>(entity =>
            {
                entity.HasKey(e => new { e.Mnid, e.Nmrid })
                    .HasName("tblMN$PrimaryKey");

                entity.ToTable("tblMN");

                entity.HasIndex(e => e.Mnid)
                    .HasName("tblMN${58F5E5B9-3552-4441-8E08-55B12448DD25}");

                entity.HasIndex(e => e.Nmrid)
                    .HasName("tblMN$NMRID");

                entity.Property(e => e.Mnid).HasColumnName("MNID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.chu2m).HasColumnName("chu2m");
                entity.Property(e => e.chu2f).HasColumnName("chu2f");
                entity.Property(e => e.refbyCHW).HasColumnName("refbyChw");
                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Mn)
                    .WithMany(p => p.TblMn)
                    .HasForeignKey(d => d.Mnid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblMN_tlkpMN");

                entity.HasOne(d => d.Nmr)
                    .WithMany(p => p.TblMn)
                    .HasForeignKey(d => d.Nmrid)
                    .HasConstraintName("FK_tblMN_NutritionMonthlyReport");
            });

            modelBuilder.Entity<TblOtp>(entity =>
            {
                entity.HasKey(e => new { e.Otpid, e.Nmrid })
                    .HasName("tblOTP$PrimaryKey");

                entity.ToTable("tblOTP");

                entity.HasIndex(e => e.Nmrid)
                    .HasName("tblOTP$NMRID");

                entity.HasIndex(e => e.Otpid)
                    .HasName("tblOTP$SFPID");

                entity.Property(e => e.Otpid).HasColumnName("OTPID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.Defaultreturn).HasColumnName("defaultreturn");

                entity.Property(e => e.Fromscotp).HasColumnName("fromscotp");

                entity.Property(e => e.Fromsfp).HasColumnName("fromsfp");

                entity.Property(e => e.Muac115).HasColumnName("MUAC115");

                entity.Property(e => e.Odema).HasColumnName("odema");

                entity.Property(e => e.TFemale).HasColumnName("tFemale");

                entity.Property(e => e.TMale).HasColumnName("tMale");

                entity.Property(e => e.Totalbegin).HasColumnName("totalbegin");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.Property(e => e.Z3score).HasColumnName("z3score");

                entity.HasOne(d => d.Nmr)
                    .WithMany(p => p.TblOtp)
                    .HasForeignKey(d => d.Nmrid)
                    .HasConstraintName("tblOTP${B1E91814-65DA-4F6F-9B61-FF55C75C6E44}");

                entity.HasOne(d => d.Otp)
                    .WithMany(p => p.TblOtp)
                    .HasForeignKey(d => d.Otpid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblOTP_tlkpOTPTFU");
            });

            modelBuilder.Entity<TblOtptfu>(entity =>
            {
                entity.HasKey(e => new { e.Otptfuid, e.Nmrid })
                    .HasName("tblOTPTFU$PrimaryKey");

                entity.ToTable("tblOTPTFU");

                entity.HasIndex(e => e.Nmrid)
                    .HasName("tblOTPTFU$NMRID");

                entity.HasIndex(e => e.Otptfuid)
                    .HasName("tblOTPTFU$SFPID");

                entity.Property(e => e.Otptfuid).HasColumnName("OTPTFUID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.Defaultreturn).HasColumnName("defaultreturn");

                entity.Property(e => e.Fromscotp).HasColumnName("fromscotp");

                entity.Property(e => e.Fromsfp).HasColumnName("fromsfp");

                entity.Property(e => e.Muac115).HasColumnName("MUAC115");

                entity.Property(e => e.Odema).HasColumnName("odema");

                entity.Property(e => e.TFemale).HasColumnName("tFemale");

                entity.Property(e => e.TMale).HasColumnName("tMale");

                entity.Property(e => e.Totalbegin).HasColumnName("totalbegin");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.Property(e => e.Z3score).HasColumnName("z3score");

                entity.HasOne(d => d.Nmr)
                    .WithMany(p => p.TblOtptfu)
                    .HasForeignKey(d => d.Nmrid)
                    .HasConstraintName("tblOTPTFU${DFECED04-52F4-4144-96A7-63D61F5F34B5}");

                entity.HasOne(d => d.Otptfu)
                    .WithMany(p => p.TblOtptfu)
                    .HasForeignKey(d => d.Otptfuid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblOTPTFU_tlkpOTPTFU");
            });


            modelBuilder.Entity<TblStockIpt>(entity =>
            {
                entity.HasKey(e => new { e.SstockId, e.Nmrid })
                    .HasName("tblSstock$PrimaryKey");

                entity.ToTable("tblStock_ipt");

                entity.HasIndex(e => e.Nmrid)
                    .HasName("tblSstock$NMRID");

                entity.HasIndex(e => e.SstockId)
                    .HasName("tblSstock$stockID1");

                entity.Property(e => e.SstockId).HasColumnName("sstockID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.Loss).HasColumnType("decimal");
                entity.Property(e => e.Expired).HasColumnType("decimal");
                entity.Property(e => e.Used).HasColumnType("decimal");
                entity.Property(e => e.Damaged).HasColumnType("decimal");
                entity.Property(e => e.Openingbalance).HasColumnType("decimal");


                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Sstock)
                    .WithMany(p => p.TblStockIpt)
                    .HasForeignKey(d => d.SstockId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblSstock_tlkpSstock");
            });

            modelBuilder.Entity<TblStockOtp>(entity =>
            {
                entity.HasKey(e => new { e.SstockotpId, e.Nmrid })
                    .HasName("tblSstock_otp$PrimaryKey");

                entity.ToTable("tblStock_otp");

                entity.HasIndex(e => e.Nmrid)
                    .HasName("tblSstock_otp$NMRID");

                entity.HasIndex(e => e.SstockotpId)
                    .HasName("tblSstock_otp$stockID1");

                entity.Property(e => e.SstockotpId).HasColumnName("sstockotpID");

                entity.Property(e => e.Nmrid)
                    .HasColumnName("NMRID")
                    .HasMaxLength(100);

                entity.Property(e => e.Loss).HasColumnType("decimal");

                entity.Property(e => e.Openingbalance).HasColumnType("decimal");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Sstockotp)
                    .WithMany(p => p.TblStockOtp)
                    .HasForeignKey(d => d.SstockotpId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblSstock_otp_tlkpSstock");
            });

            modelBuilder.Entity<TblkpStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK_tblkpStatus");

                entity.ToTable("tblkpStatus");

                entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .ValueGeneratedNever();

                entity.Property(e => e.StatusDescription).HasMaxLength(50);
            });

            modelBuilder.Entity<TlkpForms>(entity =>
            {
                entity.HasKey(e => e.FormId)
                    .HasName("PK_tlkpForms");

                entity.ToTable("tlkpForms");

                entity.Property(e => e.FormId)
                    .HasColumnName("FormID")
                    .ValueGeneratedNever();

                entity.Property(e => e.FormName).HasMaxLength(100);
            });

            modelBuilder.Entity<TlkpFstock>(entity =>
            {
                entity.HasKey(e => e.StockId)
                    .HasName("tlkpFstock$stockID");

                entity.ToTable("tlkpFstock");

                entity.Property(e => e.StockId).HasColumnName("stockID");

                entity.Property(e => e.Active).HasDefaultValueSql("0");

                entity.Property(e => e.DistAmountKg).HasColumnType("decimal");

                entity.Property(e => e.Item).HasMaxLength(255);
            });

            modelBuilder.Entity<TlkpIycf>(entity =>
            {
                entity.HasKey(e => e.Iycfid)
                    .HasName("tlkpIYCF$SFPID");

                entity.ToTable("tlkpIYCF");

                entity.Property(e => e.Iycfid).HasColumnName("IYCFID");

                entity.Property(e => e.Active).HasDefaultValueSql("0");

                entity.Property(e => e.CauseConsultation)
                    .HasColumnName("cause_consultation")
                    .HasMaxLength(255);

                entity.Property(e => e.CauseShortName).HasMaxLength(255);
            });

            modelBuilder.Entity<TlkpMn>(entity =>
            {
                entity.HasKey(e => e.Mnid)
                    .HasName("tlkpMN$PrimaryKey");

                entity.ToTable("tlkpMN");

                entity.Property(e => e.Mnid)
                    .HasColumnName("MNID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Active).HasDefaultValueSql("0");

                entity.Property(e => e.Mnitems)
                    .HasColumnName("MNitems")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<TlkpOtptfu>(entity =>
            {
                entity.HasKey(e => e.Otptfuid)
                    .HasName("tlkpOTPTFU$SFPID");

                entity.ToTable("tlkpOTPTFU");

                entity.Property(e => e.Otptfuid).HasColumnName("OTPTFUID");

                entity.Property(e => e.Active).HasDefaultValueSql("0");

                entity.Property(e => e.AgeGroup).HasMaxLength(255);
            });

            modelBuilder.Entity<TlkpSfp>(entity =>
            {
                entity.HasKey(e => e.Sfpid)
                    .HasName("tlkpSFP$SFPID");

                entity.ToTable("tlkpSFP");

                entity.Property(e => e.Sfpid).HasColumnName("SFPID");

                entity.Property(e => e.Active).HasDefaultValueSql("0");

                entity.Property(e => e.AgeGroup).HasMaxLength(255);
            });

            modelBuilder.Entity<TlkpSstock>(entity =>
            {
                entity.HasKey(e => e.SstockId)
                    .HasName("tlkpSstock$stockID");

                entity.ToTable("tlkpSstock");

                entity.Property(e => e.SstockId).HasColumnName("sstockID");

                entity.Property(e => e.Active).HasDefaultValueSql("0");

                entity.Property(e => e.Item).HasMaxLength(255);
            });
            modelBuilder.Entity<Samreq>(entity =>
           {
               entity.HasKey(e => e.Rid)
                   .HasName("PK_SAMReq");

               entity.ToTable("SAMReq");

               entity.HasIndex(e => new { e.ProvCode, e.ImpCode, e.Year, e.Month })
                   .HasName("UQ__SAMReq__E61DE2A759B61604")
                   .IsUnique();

               entity.Property(e => e.Rid).HasColumnName("RID");

               entity.Property(e => e.Chc).HasColumnName("CHC");

               entity.Property(e => e.Dh).HasColumnName("DH");

               entity.Property(e => e.Mht).HasColumnName("MHT");

               entity.Property(e => e.Ph).HasColumnName("PH");

               entity.Property(e => e.ProvCode)
                   .IsRequired()
                   .HasMaxLength(50);

               entity.Property(e => e.ReqBy).HasMaxLength(50);

               entity.Property(e => e.Shc).HasColumnName("SHC");

               entity.Property(e => e.UpdateDate).HasColumnType("date");

               entity.Property(e => e.UserName)
                   .IsRequired()
                   .HasMaxLength(50);
           });

            modelBuilder.Entity<SamreqDetails>(entity =>
            {
                entity.ToTable("SAMReqDetails");

                entity.HasIndex(e => new { e.Rid, e.SupplyId, e.FormName })
                    .HasName("UQ__SAMReqDe__12B320735C79634D")
                    .IsUnique();

                entity.Property(e => e.AdjustmentComment).HasMaxLength(255);

                entity.Property(e => e.FormName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.Property(e => e.UpdateDate).HasColumnType("date");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.R)
                    .WithMany(p => p.SamreqDetails)
                    .HasForeignKey(d => d.Rid)
                    .HasConstraintName("FK_SAMReqDetails_SAMReq");

                entity.HasOne(d => d.SId)
                    .WithMany(p => p.SamreqDetails)
                    .HasForeignKey(d => d.SupplyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblSstock_tlkpSstock");
            });

            modelBuilder.Entity<Mamreq>(entity =>
                        {
                            entity.HasKey(e => e.Rid)
                                .HasName("PK_MAMReq");

                            entity.ToTable("MAMReq");

                            entity.HasIndex(e => new { e.ProvCode, e.ImpCode, e.Year, e.Month })
                                .HasName("UQ__MAMReq__98EDC49B9F16E08E")
                                .IsUnique();

                            entity.Property(e => e.Rid).HasColumnName("RID");

                            entity.Property(e => e.Chc).HasColumnName("CHC");

                            entity.Property(e => e.Dh).HasColumnName("DH");

                            entity.Property(e => e.Mht).HasColumnName("MHT");

                            entity.Property(e => e.Ph).HasColumnName("PH");

                            entity.Property(e => e.ProvCode)
                                .IsRequired()
                                .HasMaxLength(50);

                            entity.Property(e => e.ReqBy).HasMaxLength(50);

                            entity.Property(e => e.Shc).HasColumnName("SHC");

                            entity.Property(e => e.UpdateDate).HasColumnType("date");

                            entity.Property(e => e.UserName)
                                .IsRequired()
                                .HasMaxLength(50);
                        });

            modelBuilder.Entity<MamreqDetails>(entity =>
            {
                entity.ToTable("MAMReqDetails");

                entity.HasIndex(e => new { e.Rid, e.SupplyId, e.FormName })
                    .HasName("UQ__MAMReqDe__3D3297F9B5B94ECB")
                    .IsUnique();

                entity.Property(e => e.AdjustmentComment).HasMaxLength(255);

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.HasOne(d => d.R)
                    .WithMany(p => p.MamreqDetails)
                    .HasForeignKey(d => d.Rid)
                    .HasConstraintName("FK_MAMReqDetails_MAMReq");

                entity.HasOne(d => d.SId)
                    .WithMany(p => p.MamreqDetails)
                    .HasForeignKey(d => d.SupplyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Mam_supply");

            });


            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenant");

                entity.HasKey(e => e.Id)
                    .HasName("PK_tenant");
                entity.Property(e => e.Name).HasMaxLength(255);


            });

            modelBuilder.Entity<YearFilter>(entity =>
            {
                entity.HasKey(e => e.Facility);
                entity.Property(e => e.YearFrom).HasColumnName("YearFrom");
                entity.Property(e => e.YearTo).HasColumnName("YearTo");
            });
            modelBuilder.Entity<ProvinceFilter>(entity =>
            {
                entity.HasKey(e => e.Implementer);
                entity.Property(e => e.ProvCode).HasColumnName("ProvCode");
                entity.Property(e => e.ProvName).HasColumnName("ProvName");
            });
            modelBuilder.Entity<ImpFilter>(entity =>
            {
                entity.HasKey(e => e.ImpCode);
                entity.Property(e => e.Implementer).HasColumnName("Implementer");
            });

            modelBuilder.Entity<Feedback>(entity =>
                      {
                          entity.HasKey(e => e.Id)
                              .HasName("PK_Feedback");

                          entity.ToTable("Feedback");

                          entity.Property(e => e.Nmrid)
                              .HasColumnName("NMRID")
                              .HasMaxLength(100);
                          entity.Property(e => e.CommentedBy).HasMaxLength(255);
                          entity.Property(e => e.Message).HasMaxLength(255);
                          entity.HasOne(d => d.Nmr)
                              .WithMany(p => p.Feedback)
                              .HasForeignKey(d => d.Nmrid)
                              .HasConstraintName("FK_Feedback_NMR");
                          entity.Property(e => e.CommentDate);
                      });

            modelBuilder.Entity<EmrImamServices>(entity =>
            {
                entity.HasKey(e => new { e.IndicatorId, e.ErnmrId })
                    .HasName("PK_EmrIamIndicators");

                entity.ToTable("EmrImamServices");

                entity.HasIndex(e => e.ErnmrId)
                    .HasName("EmrIamIndicators$ErnmrId");

                entity.HasIndex(e => e.IndicatorId)
                    .HasName("EmrIamIndicators$IndicatorId");

                entity.Property(e => e.IndicatorId).HasColumnName("IndicatorId");

                entity.Property(e => e.ErnmrId)
                    .HasColumnName("ErnmrId");

                entity.Property(e => e.Male).HasColumnType("int");

                entity.Property(e => e.Female).HasColumnType("int");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Ernmr)
                    .WithMany(p => p.EmrImamServices)
                    .HasForeignKey(d => d.ErnmrId)
                    .HasConstraintName("FK_Ernmr_EmrImamServices");

                entity.HasOne(d => d.tlkpEmrIndicators)
                    .WithMany(p => p.EmrImamServices)
                    .HasForeignKey(d => d.IndicatorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Ernmr_TlkpIndicators");
            });

            modelBuilder.Entity<EmrIndicators>(entity =>
            {
                entity.HasKey(e => new { e.IndicatorId, e.ErnmrId })
                    .HasName("PK_EmrIndicators");

                entity.ToTable("EmrIndicators");

                entity.HasIndex(e => e.ErnmrId)
                    .HasName("EmrIndicators$ErnmrId");

                entity.HasIndex(e => e.IndicatorId)
                    .HasName("EmrIndicators$IndicatorId");

                entity.Property(e => e.IndicatorId).HasColumnName("IndicatorId");

                entity.Property(e => e.ErnmrId)
                    .HasColumnName("ErnmrId");

                entity.Property(e => e.Male).HasColumnType("int");

                entity.Property(e => e.Female).HasColumnType("int");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Ernmr)
                    .WithMany(p => p.EmrIndicators)
                    .HasForeignKey(d => d.ErnmrId)
                    .HasConstraintName("FK_Ernmr_EmrIndicators");

                entity.HasOne(d => d.lkpEmrIndicators)
                    .WithMany(p => p.EmrIndicators)
                    .HasForeignKey(d => d.IndicatorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Ernmr_lkpIndicators");
            });

            modelBuilder.Entity<ERFacilities>(entity =>
            {
                entity.HasKey(e => e.FacilityId)
                    .HasName("ERFacilities$PrimaryKey");
                entity.ToTable("ERFacilities");
                entity.HasIndex(e => e.DistCode)
                    .HasName("FK_FacilityInfo_Districts_DistCode2");
                entity.HasIndex(e => e.FacilityType)
                    .HasName("FK_FacilityInfo_FacilityTypes_FacilityType2");

                entity.Property(e => e.FacilityId)
                    .HasColumnName("FacilityID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DistCode).HasMaxLength(50);
                entity.Property(e => e.ProvCode).HasMaxLength(2);
                entity.Property(e => e.DateEstablished).HasColumnType("date");
                entity.Property(e => e.FacilityName).HasMaxLength(255);
                entity.Property(e => e.FacilityNameDari).HasMaxLength(255);
                entity.Property(e => e.FacilityNamePashto).HasMaxLength(255);
                entity.Property(e => e.Lat).HasColumnName("LAT");
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.LocationDari).HasMaxLength(100);
                entity.Property(e => e.LocationPashto).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(2);
                entity.Property(e => e.Lon).HasColumnName("LON");
                entity.Property(e => e.Implementer).HasColumnName("Implementer");
            });

            modelBuilder.Entity<tlkpEmrIndicators>(entity =>
            {
                entity.HasKey(e => e.IndicatorId)
                    .HasName("PK_lkpIndicators");

                entity.ToTable("tlkpEmrIndicators");

                entity.Property(e => e.IndicatorId).HasColumnName("IndicatorId");
                entity.Property(e => e.IndicatorName).HasMaxLength(250);
                entity.Property(e => e.Type).HasColumnName("Type");
            });


            modelBuilder.Entity<LkpCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK_LkpCategory");

                entity.ToTable("LkpCategory");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryId");
                entity.Property(e => e.CategoryName).HasMaxLength(200);
            });
            modelBuilder.Entity<lkpThematicArea>(entity =>
            {
                entity.HasKey(e => e.ThemeId)
                    .HasName("PK_lkpThematicArea");

                entity.ToTable("lkpThematicArea");

                entity.Property(e => e.ThemeId).HasColumnName("ThemeId");
                entity.Property(e => e.ThematicArea).HasMaxLength(200);
            });

            modelBuilder.Entity<LkpDisaggregation>(entity =>
            {
                entity.HasKey(e => e.DisaggregId)
                    .HasName("PK_LkpDisaggregation");

                entity.ToTable("LkpDisaggregation");

                entity.Property(e => e.DisaggregId).HasColumnName("DisaggregId");
                entity.Property(e => e.CategoryId).HasColumnName("CategoryId");
                entity.Property(e => e.Ordno).HasColumnName("Ordno");
                entity.Property(e => e.Disaggregation).HasMaxLength(250);
            });

            //modelBuilder.Entity<SurveyResults>(entity =>
            //{
            //    entity.HasKey(e => new { e.IndResultId })
            //        .HasName("PK_SurveyResults");

            //    entity.ToTable("SurveyResults");

            //    entity.HasIndex(e => e.DisaggregId)
            //        .HasName("FK_SurveyResults_LkpDisaggregation_DisaggregId");

            //    entity.HasIndex(e => e.ThemeId)
            //        .HasName("FK_SurveyResults_lkpThematicArea_ThemeId");

            //    entity.Property(e => e.IndResultId).HasColumnName("IndResultId");

            //    entity.Property(e => e.DisaggregId).HasColumnName("DisaggregId");
            //    entity.Property(e => e.ThemeId).HasColumnName("ThemeId");

            //    entity.Property(e => e.IndicatorId).HasColumnName("IndicatorId").HasMaxLength(250);
            //    entity.Property(e => e.IndicatorValue).HasColumnName("IndicatorValue").HasColumnType("falot"); ;
            //    entity.Property(e => e.CINational).HasColumnName("CINational").HasMaxLength(50);
            //    entity.Property(e => e.Year).HasColumnName("Year");
            //    entity.Property(e => e.Month).HasColumnName("Month");
            //    entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            //    entity.Property(e => e.TenantId).HasColumnName("TenantId");
            //    entity.Property(e => e.UserName).HasMaxLength(50);

            //    entity.HasOne(d => d.LkpDisaggregations)
            //        .WithMany(p => p.SurveyResults)
            //        .HasForeignKey(d => d.DisaggregId)
            //        .OnDelete(DeleteBehavior.Restrict)
            //        .HasConstraintName("FK_SurveyResults_LkpDisaggregation_DisaggregId");

            //    entity.HasOne(d => d.LkpThematicAreas)
            //        .WithMany(p => p.SurveyResults)
            //        .HasForeignKey(d => d.ThemeId)
            //        .HasConstraintName("FK_SurveyResults_lkpThematicArea_ThemeId");

            //    entity.HasOne(d => d.SurveyInfoNav)
            //        .WithMany(p => p.SurveyResults)
            //        .HasForeignKey(d => d.SurveyId)
            //        .HasConstraintName("FK_SurveyInfo_SurveyResults_SurveyId");
            //});

            modelBuilder.Entity<hmisindicators>(entity =>
            {
                entity.HasKey(e => e.IndicatorId)
                    .HasName("PK_HMISIndicators");

                entity.ToTable("HMISIndicators");

                entity.Property(e => e.IndicatorId).HasColumnName("IndicatorId");
                entity.Property(e => e.IndicatorDescription).HasMaxLength(250);
                entity.Property(e => e.IndDataSource).HasMaxLength(250);
                entity.Property(e => e.IndCaluculation).HasMaxLength(250);
                entity.Property(e => e.IndType).HasMaxLength(1);
            });
            modelBuilder.Entity<lkpChecklist>(entity =>
            {
                entity.HasKey(e => e.IntId)
                    .HasName("PK_lkpChecklist");

                entity.ToTable("lkpChecklist");

                entity.Property(e => e.IntId).HasColumnName("IntId");
                entity.Property(e => e.OrderId);
                entity.Property(e => e.Stringorder).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(250);
                entity.Property(e => e.DescriptionDari).HasMaxLength(250);                
                entity.Property(e => e.Active);
                entity.Property(e => e.Type).HasMaxLength(50);

            });

            modelBuilder.Entity<ChkCGM>(entity =>
            {
                entity.HasKey(e => new { e.IndId, e.ChkId })
                    .HasName("PK_ChkCGM");

                entity.ToTable("ChkCGM");

                entity.HasIndex(e => e.IndId)
                    .HasName("FK_ChkCGM_lkpChecklist_IndId");

                entity.HasIndex(e => e.ChkId)
                    .HasName("FK_ChkCGM_Checklist_ChkId");

                entity.Property(e => e.IndId).HasColumnName("IndId");

                entity.Property(e => e.ChkId).HasColumnName("ChkId");

                entity.Property(e => e.Response).HasColumnName("Response");
                entity.Property(e => e.NResponse).HasColumnName("NResponse");
                entity.Property(e => e.UserName).HasColumnName("UserName").HasMaxLength(50);
                entity.Property(e => e.UpdateDate).HasColumnType("datetime");
                entity.Property(e => e.Tenant).HasColumnType("int");


                entity.HasOne(d => d.GetLkpChecklist)
                    .WithMany(p => p.ChkCGMs)
                    .HasForeignKey(d => d.IndId)
                    .HasConstraintName("FK_ChkCGM_lkpChecklist_IndId");

                entity.HasOne(d => d.GetChecklist)
                    .WithMany(p => p.ChkCGMs)
                    .HasForeignKey(d => d.ChkId)
                    .HasConstraintName("FK_ChkCGM_Checklist_ChkId");
            });

            modelBuilder.Entity<TextValue>()
                .HasKey(m => new { m.FieldId, m.ReportId });

            modelBuilder.Entity<NumberValue>()
                .HasKey(m => new { m.FieldId, m.ReportId });

            modelBuilder.Entity<DateValue>()
                .HasKey(m => new { m.FieldId, m.ReportId });

            modelBuilder.Entity<VDateValue>()
                .HasKey(m => new { m.FieldId, m.ReportsViewId });
        }

        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<Districts> Districts { get; set; }
        public virtual DbSet<FacilityInfo> FacilityInfo { get; set; }
        public virtual DbSet<FacilityTypes> FacilityTypes { get; set; }
        public virtual DbSet<Implementers> Implementers { get; set; }
        public virtual DbSet<LkpHfstatus> LkpHfstatus { get; set; }
        public virtual DbSet<Nmr> Nmr { get; set; }
        public virtual DbSet<Provinces> Provinces { get; set; }
        public virtual DbSet<nmrsubmission> nmrsubmission { get; set; }
        public virtual DbSet<monthlysubmission> monthlysubmission { get; set; }
        public virtual DbSet<checkcompleteness> checkcompleteness { get; set; }
        public virtual DbSet<submissionRes> submissionRes { get; set; }
        public virtual DbSet<TblFeedback> TblFeedback { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }
        public virtual DbSet<TblFstock> TblFstock { get; set; }
        public virtual DbSet<TblIycf> TblIycf { get; set; }
        public virtual DbSet<TblMam> TblMam { get; set; }
        public virtual DbSet<TblMn> TblMn { get; set; }
        public virtual DbSet<TblOtp> TblOtp { get; set; }
        public virtual DbSet<TblOtptfu> TblOtptfu { get; set; }
        public virtual DbSet<TblStockIpt> TblStockIpt { get; set; }
        public virtual DbSet<TblStockOtp> TblStockOtp { get; set; }
        public virtual DbSet<TblkpStatus> TblkpStatus { get; set; }
        public virtual DbSet<TblQnr> TblQnr { get; set; }
        public virtual DbSet<Samreq> Samreq { get; set; }
        public virtual DbSet<SamreqDetails> SamreqDetails { get; set; }
        public virtual DbSet<Mamreq> Mamreq { get; set; }
        public virtual DbSet<MamreqDetails> MamreqDetails { get; set; }
        public virtual DbSet<TlkpForms> TlkpForms { get; set; }
        public virtual DbSet<TlkpFstock> TlkpFstock { get; set; }
        public virtual DbSet<TlkpIycf> TlkpIycf { get; set; }
        public virtual DbSet<TlkpMn> TlkpMn { get; set; }
        public virtual DbSet<TlkpOtptfu> TlkpOtptfu { get; set; }
        public virtual DbSet<TlkpSfp> TlkpSfp { get; set; }
        public virtual DbSet<TlkpSstock> TlkpSstock { get; set; }
        public virtual DbSet<TempFacilities> TempFacilities { get; set; }
        public virtual DbSet<YearFilter> YearFilter { get; set; }
        public virtual DbSet<ProvinceFilter> ProvinceFilter { get; set; }
        public virtual DbSet<ImpFilter> ImpFilter { get; set; }
        public virtual DbSet<provincemonthly> provincemonthly {get;set;}
        public virtual DbSet<statReports> statReports {get;set;}
        public virtual DbSet<reportsubmission> reportsubmission { get; set; }
        public virtual DbSet<vsamavail> vsamavail { get; set; }
        public virtual DbSet<vmamavail> vmamavail { get; set; }
        public virtual DbSet<Formatmamreport> Formatmamreports { get; set; }
        public virtual DbSet<mamcommon> Mamcommons { get; set; }
        public virtual DbSet<Formatmamstockreport> Formatmamstockreports { get; set; }
        public virtual DbSet<FormatYear> FormatYears { get; set; }
        public virtual DbSet<vHFImplementer> VHFImplementers { get; set; }
        public virtual DbSet<vDistImplementers> VDistImplementers { get; set; }
        public virtual DbSet<vProvImplementers> VProvImplementers { get; set; }
        public virtual DbSet<ERFacilities> ERFacilities { get; set; }
        public virtual DbSet<Ernmr> Ernmr { get; set; }
        public virtual DbSet<tlkpEmrIndicators> TlkpEmrIndicators { get; set; }
        public virtual DbSet<EmrImamServices> EmrImamServices { get; set; }
        public virtual DbSet<EmrIndicators> EmrIndicators { get; set; }
        public virtual DbSet<SurveyInfo> SurInfo { get; set; }
        public virtual DbSet<LkpCategory> LkpCategories { get; set; }
        public virtual DbSet<lkpThematicArea> LkpThematicAreas { get; set; }
        public virtual DbSet<LkpDisaggregation> LkpDisaggregations { get; set; }
        public virtual DbSet<hmisindicators> Hmisindicators { get; set; }
        public virtual DbSet<SurveyResults> SurveyResults { get; set; }
        public virtual DbSet<lkpChecklist> GetLkpChecklists { get; set; }
        public virtual DbSet<TemphmisindicatorValues> TemphmisindicatorValues { get; set; }
        public virtual DbSet<HMISIndicatorValues> HMISIndicatorValues { get; set; }
        public virtual DbSet<TempDistPopulation> TempDistPopulation { get; set; }
        public virtual DbSet<DistPopulation> DistPopulation { get; set; }
        public virtual DbSet<masteremails> masteremails { get; set; }
        public virtual DbSet<tlkpbiweekly> tlkpbiweekly { get; set; }
        public virtual DbSet<Yearlist> Yearlist { get; set; }
        public virtual DbSet<lkpSurveyIndicators> lkpSurveyIndicators { get; set; }
        public virtual DbSet<Apistore> Apistore { get; set; }
        public virtual DbSet<Notehelpers> Notehelpers { get; set; }
        public virtual DbSet<vmFacilityimps> vmFacilityimps { get; set; }

        //SCM
        public virtual DbSet<scmPOC> ScmPOCs { get; set; }
        public virtual DbSet<scmRequest> scmRequest { get; set; }
        public virtual DbSet<scmRegions> scmRegions { get; set; }
        public virtual DbSet<scmWarehouses> scmWarehouses { get; set; }
        public virtual DbSet<TempRequest> TempRequest { get; set; }
        public virtual DbSet<scmHFRequest> ScmHFRequest { get; set; }
        public virtual DbSet<scmHFReqDetails> scmHFReqDetails { get; set; }
        public virtual DbSet<scmIPRequest> scmIPRequests { get; set; }
        public virtual DbSet<Lkpwhlevels> Lkpwhlevels { get; set; }
        public virtual DbSet<scmStocks> scmStocks { get; set; }
        public virtual DbSet<scmDistributionsIP> scmDistributionsIP { get; set; }
        public virtual DbSet<scmStockBalance> scmStockBalance { get; set; }
        public virtual DbSet<scmUsersset> scmUsersset { get; set; }
        public virtual DbSet<scmRounds> scmRounds { get; set; }
        public virtual DbSet<scmDistributionMain> scmDistributionMain { get; set; }
        public virtual DbSet<scmEmail> scmEmail { get; set; }
        public virtual DbSet<scmDocs> scmDocs { get; set; }
        public virtual DbSet<scmDoctypes> scmDoctypes { get; set; }
        public virtual DbSet<scmEstsubmission> scmEstsubmission { get; set; }
        public virtual DbSet<scmEstNotification> scmEstNotification { get; set; }
        public virtual DbSet<scmNotificationlist> scmNotificationlist { get; set; }
        public virtual DbSet<vmEstNotification> vmEstNotification { get; set; }
        public virtual DbSet<scmDistributionFacilities> scmDistributionFacilities { get; set; }
        public virtual DbSet<scmWasteTypes> scmWasteTypes { get; set; }
        public virtual DbSet<scmWastages> scmWastages { get; set; }
        public virtual DbSet<scmStockaverage> scmStockaverage { get; set; }
        public virtual DbSet<scmAveragelevel> scmAveragelevel { get; set; }
        public virtual DbSet<scmRequeststatus> scmRequeststatus { get; set; }
        public virtual DbSet<scmRequesttype> scmRequesttype { get; set; }
        public virtual DbSet<scmMonths> scmMonths { get; set; }
        public virtual DbSet<scmmailgroup> scmmailgroup { get; set; }
        public virtual DbSet<scmRequestReason> scmRequestReason { get; set; }
        public virtual DbSet<scmipRequestConfirmation> scmipRequestConfirmation { get; set; }
        public virtual DbSet<vscmRequestList> vscmRequestList { get; set; }
        public virtual DbSet<scmContacts> scmContacts { get; set; }
        public virtual DbSet<scmIPAcknowledgement> scmIPAcknowledgement { get; set; }
        public virtual DbSet<scmHFsAcknowledgement> scmHFsAcknowledgement { get; set; }
        public virtual DbSet<scmTransfers> scmTransfers { get; set; }
        public virtual DbSet<vscmDistributiontransfer> vscmDistributiontransfer { get; set; }
        public virtual DbSet<scmRequeststatusitems> scmRequeststatusitems { get; set; }
        public virtual DbSet<scmRequeststage> scmRequeststage { get; set; }
        public virtual DbSet<rptIPStockmovement> rptIPStockmovement { get; set; }
        public virtual DbSet<rptIPStockmovementdetails> rptIPStockmovementdetails { get; set; }
        public virtual DbSet<scmRecalldisposal> scmRecalldisposal { get; set; }
        //public virtual DbSet<scmFiles> scmFiles { get; set; }

        //Views
        public virtual DbSet<vscmRequeststatus> vscmRequeststatus { get; set; }
        public virtual DbSet<scmrptRequestpivot> scmrptRequestpivot { get; set; }
        public virtual DbSet<scmdashrequest> scmdashrequest { get; set; }
        public virtual DbSet<scmdashdistmain> scmdashdistmain { get; set; }
        public virtual DbSet<scmdashsubmission> scmdashsubmission { get; set; }
        public virtual DbSet<scmdashrequestip> scmdashrequestip { get; set; }
        public virtual DbSet<vscmrequests> vscmrequests { get; set; }
        public virtual DbSet<vscmiprequest> vscmiprequest { get; set; }
        public virtual DbSet<vscmstockwastages> vscmstockwastages { get; set; }



        //HP Checklist for Monitoring the Quality of Nutrition Services at Health Posts level
        public virtual DbSet<HpMonitoring> HpMonitoring { get; set; }
        public virtual DbSet<HpCapacityBuilding> HpCapacityBuilding { get; set; }
        public virtual DbSet<HpCbnpKits> HpCbnpKits { get; set; }
        public virtual DbSet<HpCommunityNutritionPlan> HpCommunityNutritionPlan { get; set; }
        public virtual DbSet<HpMonitoringlkp> HpMonitoringlkp { get; set; }
        public virtual DbSet<HpRecommendations> HpRecommendations { get; set; }
        public virtual DbSet<HpResponses> HpResponses { get; set; }
        public virtual DbSet<HpScreening> HpScreening { get; set; }
        public virtual DbSet<vHpMonitoring> vHpMonitoring { get; set; }

        //GLM
        public DbSet<DataForm> DataForms { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldOption> FieldOptions { get; set; }
        public DbSet<TextValue> TextValues { get; set; }
        public DbSet<NumberValue> NumberValues { get; set; }
        public DbSet<DateValue> DateValues { get; set; }
        //public DbSet<VDateValue> VDateValues { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportsView> ReportsView { get; set; }
    }
}