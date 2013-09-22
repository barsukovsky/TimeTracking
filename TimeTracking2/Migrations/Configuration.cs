namespace TimeTracking2.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebMatrix.WebData;
    using System.Web.Security;

    internal sealed class Configuration : DbMigrationsConfiguration<TimeTracking2.Models.EFDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TimeTracking2.Models.EFDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            WebSecurity.InitializeDatabaseConnection("EFDbContext", "UserProfiles", "UserId", "UserName", autoCreateTables: true);
            if (!Roles.RoleExists("Administrator"))
            {
                Roles.CreateRole("Administrator");
            }
            if (!WebSecurity.UserExists("admin"))
            {
                WebSecurity.CreateUserAndAccount("admin", "secret",
                    new
                    {
                        FirstName = "Админ",
                        LastName = "Вселенский"
                    });
            }
            if (!Roles.GetRolesForUser("admin").Contains("Administrator"))
            {
                Roles.AddUsersToRoles(new[] { "admin" }, new[] { "Administrator" });
            }
            if (!WebSecurity.UserExists("user1"))
            {
                WebSecurity.CreateUserAndAccount("user1", "user1",
                    new
                    {
                        FirstName = "Юзер",
                        MiddleName = "Кузьмич",
                        LastName = "Первый",
                        Appointment = "Водитель",
                        CurrentHourlyRate = "30"
                    });
            }
            if (!WebSecurity.UserExists("user2"))
            {
                WebSecurity.CreateUserAndAccount("user2", "user2",
                    new
                    {
                        FirstName = "Юзер",
                        MiddleName = "Петрович",
                        LastName = "Второй",
                        Appointment = "Слесарь",
                        CurrentHourlyRate = "25"
                    });
            }
            if (!WebSecurity.UserExists("user3"))
            {
                WebSecurity.CreateUserAndAccount("user3", "user3",
                    new
                    {
                        FirstName = "Юзер",
                        MiddleName = "Михалыч",
                        LastName = "Третий",
                        Appointment = "Механик",
                        CurrentHourlyRate = "35"
                    });
            }

            var ids = new []
            {
                WebSecurity.GetUserId("user1"),
                WebSecurity.GetUserId("user2"),
                WebSecurity.GetUserId("user3")
            };

            Random rnd = new Random();

            DateTime date = new DateTime(2010, 1, 1);
            for (int i = 0; i < 36; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    context.Reports.Add(new Models.Report
                    {
                        Year = date.Year,
                        Month = date.Month,
                        UserId = ids[j],
                        Hours = rnd.Next(50, 160),
                        HourlyRate = rnd.Next(1, 30)
                    });
                }
                date = date.AddMonths(1);
            }
        }
    }
}
