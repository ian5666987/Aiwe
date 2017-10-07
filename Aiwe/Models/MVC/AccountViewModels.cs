using System.ComponentModel.DataAnnotations;

namespace Aiwe.Models {
  public class LoginViewModel {
    [Required]
    [EmailAddress]
    [Display(Name = Aiwe.LCZ.F.LoginViewModel.Email)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = Aiwe.LCZ.F.LoginViewModel.Password)]
    public string Password { get; set; }

    [Display(Name = Aiwe.LCZ.F.LoginViewModel.RememberMe)]
    public bool RememberMe { get; set; }
  }
}
