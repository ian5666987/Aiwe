namespace Aiwe.Migrations {
  using Aibe.Helpers;
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.EntityFramework;
  using Aibe.Models.DB;
  using Models;
  using System;
  using System.Data.Entity.Migrations;
  using System.Linq;
  
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
        new IdentityRole { Name = Aibe.DH.DevRole },
        new IdentityRole { Name = Aibe.DH.MainAdminRole },
        new IdentityRole { Name = Aibe.DH.AdminRole },
        new IdentityRole { Name = "Manager" },
        new IdentityRole { Name = "Supervisor" },
        new IdentityRole { Name = "User" }
      );

      bool adminExist = context.Users.Any(t => t.UserName == Aiwe.DH.MainAdminName);
      bool devExist = context.Users.Any(t => t.UserName == Aibe.DH.DevName);
      bool sharedDevExist = context.Users.Any(t => t.UserName == Aiwe.DH.SharedDevName);
      if (!adminExist || !devExist || !sharedDevExist) {
        var userStore = new UserStore<ApplicationUser>(context);
        var userManager = new UserManager<ApplicationUser>(userStore);
        DateTime now = DateTime.Now;
        if (!devExist) {
          ApplicationUser ian = new ApplicationUser {
            FullName = Aibe.DH.DevFullName,
            UserName = Aibe.DH.DevName,
            DisplayName = Aibe.DH.DevDisplayName,
            Email = Aibe.DH.DevName,
            EmailConfirmed = true,
            LockoutEnabled = false,
            Team = "Home",
            AdminRole = Aibe.DH.DevRole,
            RegistrationDate = now,
            LastLogin = now,
          };
          userManager.Create(ian, Aibe.DH.DevPass);
          userManager.AddToRole(ian.Id, Aibe.DH.DevRole);
          UserHelper.CreateUserMap(db, ian.UserName, Aibe.DH.DevPass);
        }

        if (!adminExist) {
          ApplicationUser admin = new ApplicationUser {
            FullName = Aibe.DH.MainAdminFullName,
            UserName = Aiwe.DH.MainAdminName,
            DisplayName = Aibe.DH.MainAdminDisplayName,
            Email = Aiwe.DH.MainAdminName,
            EmailConfirmed = true,
            LockoutEnabled = false,
            Team = "Home",
            AdminRole = Aibe.DH.MainAdminRole,
            RegistrationDate = now,
            LastLogin = now,
          };
          userManager.Create(admin, Aiwe.DH.MainAdminPass);
          userManager.AddToRole(admin.Id, Aibe.DH.MainAdminRole);
          UserHelper.CreateUserMap(db, admin.UserName, Aiwe.DH.MainAdminPass);
        }

        if (!sharedDevExist) {
          ApplicationUser developer = new ApplicationUser {
            FullName = Aiwe.DH.SharedDevFullName,
            UserName = Aiwe.DH.SharedDevName,
            DisplayName = Aibe.DH.DevDisplayName,
            Email = Aiwe.DH.SharedDevName,
            EmailConfirmed = true,
            LockoutEnabled = false,
            Team = "Home",
            AdminRole = Aibe.DH.DevRole,
            RegistrationDate = now,
            LastLogin = now,
          };
          userManager.Create(developer, Aiwe.DH.SharedDevPass);
          userManager.AddToRole(developer.Id, Aibe.DH.DevRole);
          UserHelper.CreateUserMap(db, developer.UserName, Aiwe.DH.SharedDevPass);
        }

        context.SaveChanges();
      }
    }
  }
}
