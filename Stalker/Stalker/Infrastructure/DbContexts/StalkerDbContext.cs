using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stalker.Entities;
//using Stalker.Entities.EntitiesConfiguration;
using Stalker.Infrastructure.EntitiesConfiguration;

namespace Stalker.Infrastructure.DbContexts
{
    //База данных Stalker
    public class StalkerDbContext : IdentityDbContext<StalkerIdentityUser>
    {
        public StalkerDbContext() : base("name=Stalker")
        {
            Database.SetInitializer<StalkerDbContext>(new StalkerDbInit());
        }

        public static StalkerDbContext Create() => new StalkerDbContext();

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Базовый метод создает таблицы для Identity
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new UserInfoConfiguration());
            modelBuilder.Configurations.Add(new StalkerIdentityRoleConfiguration());
            modelBuilder.Configurations.Add(new ManagementConfiguration());
            modelBuilder.Configurations.Add(new DepartmentConfiguration());
            modelBuilder.Configurations.Add(new RegionConfiguration());
            modelBuilder.Configurations.Add(new CityConfiguration());
        }

        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Management> Managements { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<StalkerIdentityRole> IdentityRoles { get; set; }

        //public System.Data.Entity.DbSet<Stalker.Entities.StalkerIdentityUser> StalkerIdentityUsers { get; set; }
    }

    //Класс-инициализатор БД
    public class StalkerDbInit : CreateDatabaseIfNotExists<StalkerDbContext>
    {
        class SchemaDb
        {
            public List<Management> Managements { get; set; }
            public List<Region> Regions { get; set; }
            public List<StalkerIdentityRole> StalkerIdentityRoles { get; set; }
            public List<StalkerIdentityUser> StalkerIdentityUsers { get; set; }
        }

        //Заполняем начальными данными из файла
        protected override void Seed(StalkerDbContext context)
        {
            StalkerUserManager userManager = new StalkerUserManager(new UserStore<StalkerIdentityUser>(context));
            StalkerRoleManager roleManager = new StalkerRoleManager(new RoleStore<StalkerIdentityRole>(context));

            string storageFilePath =
                HttpContext.Current.Server.MapPath("~/Infrastructure/SeedingDatabase/LocalStorage.json");

            using (StreamReader reader = new StreamReader(storageFilePath))
            {
                string json = reader.ReadToEnd();

                SchemaDb dbFile = JsonConvert.DeserializeObject<SchemaDb>(json,
                    new IsoDateTimeConverter {DateTimeFormat = "dd/MM/yyyy"});

                //Импортируем в БД регионы и города
                foreach (Region currentRegion in dbFile.Regions)
                {
                    context.Regions.Add(currentRegion);
                    foreach (City currentCity in currentRegion.Cities)
                    {
                        currentCity.Region = currentRegion;
                        context.Cities.Add(currentCity);
                    }
                }

                //Импортируем в БД управления и отделы
                foreach (Management currentManagement in dbFile.Managements)
                {
                    context.Managements.Add(currentManagement);
                    foreach (Department currentDepartment in currentManagement.Departments)
                    {
                        currentDepartment.Management = currentManagement;
                        context.Departments.Add(currentDepartment);
                    }
                }

                //Добавление ролей
                foreach (StalkerIdentityRole role in dbFile.StalkerIdentityRoles)
                {
                    if (!roleManager.RoleExists(role.Name))
                    {
                        context.Roles.Add(role);
                    }
                }

                //Добавление пользователей
                foreach (StalkerIdentityUser currentUser in dbFile.StalkerIdentityUsers)
                {
                    if (userManager.FindByName(currentUser.UserName) == null)
                    {
                        userManager.Create(currentUser, currentUser.Password);
                        if (currentUser.UserName == "Admin")
                        {
                            string userId = (userManager.FindByName(currentUser.UserName)).Id;
                            userManager.AddToRole(userId, "Administrator");
                        }
                    }
                }
                context.SaveChanges();
            }
        }
    }
}