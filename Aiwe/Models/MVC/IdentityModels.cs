using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiwe.Models {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUser : IdentityUser {
    public string FullName { get; set; }
    public string DisplayName { get; set; }
    public string WorkingRole { get; set; }
    public string AdminRole { get; set; }
    public string Team { get; set; }
    public DateTime RegistrationDate { get; set; } = new DateTime(1999, 12, 31, 23, 59, 59); //when the user is registered, then it has its registration date
    public DateTime? LastLogin { get; set; } = new DateTime(1999, 12, 30, 23, 59, 59); //To check when was the last login of the user
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
      // Add custom user claims here
      userIdentity.AddClaim(new Claim(Aiwe.DH.UserDisplayName, DisplayName));
      return userIdentity;
    }
  }

  public class Team {
    [Key]
    [Display(Name = Aiwe.LCZ.F.Team.Id)]
    public int Id { get; set; }

    [Display(Name = Aiwe.LCZ.F.Team.Name)]
    public string Name { get; set; }
  }

  public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false) {
    }

    public static ApplicationDbContext Create() {
      return new ApplicationDbContext();
    }

    public DbSet<Team> Teams { get; set; }
  }
}