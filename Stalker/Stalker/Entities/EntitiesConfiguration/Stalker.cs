using System.Data.Entity.ModelConfiguration;
using Stalker.Entities;

//Здесь описываются сущности БД используя Fluent API
namespace Stalker.Infrastructure.EntitiesConfiguration
{
    //Профиль пользователя
    public class UserInfoConfiguration : EntityTypeConfiguration<UserInfo>
    {
        public UserInfoConfiguration()
        {
            Property(n => n.Name).HasMaxLength(50);
            Property(fam => fam.Family).HasMaxLength(50);
            Property(otch => otch.Otch).HasMaxLength(50);
            Property(d => d.DateBirth).IsOptional().HasColumnType("Date");
            Property(d => d.DateCreate).IsOptional().HasColumnType("datetime2").HasPrecision(0);
            Property(d => d.DateModified).IsOptional().HasColumnType("datetime2").HasPrecision(0);

            HasRequired(u => u.StalkerIdentityUser)
                .WithRequiredDependent(ui => ui.UserInfo);

            HasRequired(ui => ui.Management)
                .WithMany(m => m.UserInfoes)
                .HasForeignKey(ui => ui.ManagementId);

            HasOptional(ui => ui.Department)
                .WithMany(d => d.UserInfoes)
                .HasForeignKey(ui => ui.DepartmentId);

            HasRequired(ui => ui.City)
                .WithMany(c => c.UserInfoes)
                .HasForeignKey(ui => ui.CityId);

            HasRequired(ui => ui.Region)
                .WithMany(r => r.UserInfoes)
                .HasForeignKey(ui => ui.RegionId)
                .WillCascadeOnDelete(false);
                
            HasKey(e => e.StalkerIdentityUserId);
            ToTable("UserInfoes");
        }
    }

    //Роли пользователя
    public class StalkerIdentityRoleConfiguration : EntityTypeConfiguration<StalkerIdentityRole>
    {
        public StalkerIdentityRoleConfiguration()
        {
            Property(disp => disp.DisplayName).HasMaxLength(50);
        }
    }
    
    //Отделы в управлении
    public class DepartmentConfiguration : EntityTypeConfiguration<Department>
    {
        public DepartmentConfiguration()
        {
            Property(n => n.FullName).HasMaxLength(50).IsRequired();
            Property(p => p.ShortName).HasMaxLength(50);
            Property(d => d.DateCreate).IsOptional().HasColumnType("datetime2").HasPrecision(0);
            Property(d => d.DateModified).IsOptional().HasColumnType("datetime2").HasPrecision(0);

            HasRequired(dep => dep.Management)
                .WithMany(man => man.Departments)
                .HasForeignKey(dep => dep.ManagementId);
            ToTable("sl_Departments");
        }
    }

    #region Справочники
    //Справочник городов
    public class CityConfiguration : EntityTypeConfiguration<City>
    {
        public CityConfiguration()
        {
            Property(n => n.CityName).HasMaxLength(50).IsRequired();
            ToTable("sl_Cities");
        }
    }

    //Справочник регионов
    public class RegionConfiguration : EntityTypeConfiguration<Region>
    {
        public RegionConfiguration()
        {
            Property(n => n.RegionName).HasMaxLength(100).IsRequired();
            HasMany(r => r.Cities)
                .WithRequired(c => c.Region)
                .Map(m => m.MapKey("RegionId"));
            ToTable("sl_Regions");
        }
    }

    //Справочник управлений
    public class ManagementConfiguration : EntityTypeConfiguration<Management>
    {
        public ManagementConfiguration()
        {
            Property(n => n.FullName).HasMaxLength(50).IsRequired();
            Property(p => p.ShortName).HasMaxLength(50);
            Property(d => d.DateCreate).IsOptional().HasColumnType("datetime2").HasPrecision(0);
            Property(d => d.DateModified).IsOptional().HasColumnType("datetime2").HasPrecision(0);

            ToTable("sl_Managements");
        }
    }
    #endregion
}