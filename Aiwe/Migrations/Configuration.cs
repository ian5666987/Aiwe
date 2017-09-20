namespace Aiwe.Migrations {
  using Aibe.Helpers;
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.EntityFramework;
  using Aibe.Models.DB;
  using Models;
  using System;
  using System.Data.Entity.Migrations;
  using System.Linq;
  using Aibe;

  internal sealed class Configuration : DbMigrationsConfiguration<Aiwe.Models.ApplicationDbContext> {
    public Configuration() {
      AutomaticMigrationsEnabled = true;
      ContextKey = "Aiwe.Models.ApplicationDbContext";
    }

    CoreDataModel db = new CoreDataModel();

    protected override void Seed(Aiwe.Models.ApplicationDbContext context) {
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

      context.Roles.AddOrUpdate(r => r.Name,
        new IdentityRole { Name = DH.DevRole },
        new IdentityRole { Name = DH.MainAdminRole },
        new IdentityRole { Name = DH.AdminRole },
        new IdentityRole { Name = "Manager" },
        new IdentityRole { Name = "Supervisor" },
        new IdentityRole { Name = "User" }
      );

      bool adminExist = context.Users.Any(t => t.UserName == DH.MainAdminName);
      bool devExist = context.Users.Any(t => t.UserName == DH.DevName);
      bool sharedDevExist = context.Users.Any(t => t.UserName == DH.SharedDevName);
      if (!adminExist || !devExist || !sharedDevExist) {
        var userStore = new UserStore<ApplicationUser>(context);
        var userManager = new UserManager<ApplicationUser>(userStore);
        DateTime now = DateTime.Now;
        if (!devExist) {
          ApplicationUser ian = new ApplicationUser {
            FullName = DH.DevFullName,
            UserName = DH.DevName,
            DisplayName = DH.DevDisplayName,
            Email = DH.DevName,
            EmailConfirmed = true,
            LockoutEnabled = false,
            Team = "Home",
            AdminRole = DH.DevRole,
            RegistrationDate = now,
            LastLogin = now,
          };
          userManager.Create(ian, DH.DevPass);
          userManager.AddToRole(ian.Id, DH.DevRole);
          UserHelper.CreateUserMap(db, ian.UserName, DH.DevPass);
        }

        if (!adminExist) {
          ApplicationUser admin = new ApplicationUser {
            FullName = DH.MainAdminFullName,
            UserName = DH.MainAdminName,
            DisplayName = DH.MainAdminDisplayName,
            Email = DH.MainAdminName,
            EmailConfirmed = true,
            LockoutEnabled = false,
            Team = "Home",
            AdminRole = DH.MainAdminRole,
            RegistrationDate = now,
            LastLogin = now,
          };
          userManager.Create(admin, DH.MainAdminPass);
          userManager.AddToRole(admin.Id, DH.MainAdminRole);
          UserHelper.CreateUserMap(db, admin.UserName, DH.MainAdminPass);
        }

        if (!sharedDevExist) {
          ApplicationUser developer = new ApplicationUser {
            FullName = DH.SharedDevFullName,
            UserName = DH.SharedDevName,
            DisplayName = DH.DevDisplayName,
            Email = DH.SharedDevName,
            EmailConfirmed = true,
            LockoutEnabled = false,
            Team = "Home",
            AdminRole = DH.DevRole,
            RegistrationDate = now,
            LastLogin = now,
          };
          userManager.Create(developer, DH.SharedDevPass);
          userManager.AddToRole(developer.Id, DH.DevRole);
          UserHelper.CreateUserMap(db, developer.UserName, DH.SharedDevPass);
        }

        context.SaveChanges();
      }
    }
  }
}
