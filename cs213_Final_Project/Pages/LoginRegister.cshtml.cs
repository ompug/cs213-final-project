using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

public class LoginRegisterModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<LoginRegisterModel> _logger;

    public LoginRegisterModel(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        ILogger<LoginRegisterModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public LoginInputModel LoginInput { get; set; }

    [BindProperty]
    public RegisterInputModel RegisterInput { get; set; }

    public class LoginInputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterInputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        // hide any RegisterInput errors so only LoginInput validates
        ModelState.Remove(nameof(RegisterInput.Email));
        ModelState.Remove(nameof(RegisterInput.Password));
        ModelState.Remove(nameof(RegisterInput.ConfirmPassword));

        ViewData["ShowRegister"] = false;

        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(LoginInput.Email);
        if (user == null)
        {
            _logger.LogWarning("Login failed: no account for {Email}", LoginInput.Email);
            ModelState.AddModelError(nameof(LoginInput.Email), "No account found with that email.");
            return Page();
        }

        var result = await _signInManager
            .PasswordSignInAsync(LoginInput.Email, LoginInput.Password, false, lockoutOnFailure: false);

        if (result.Succeeded)
            return RedirectToPage("/Index");

        if (result.IsLockedOut)
        {
            _logger.LogWarning("Login failed: {Email} is locked out", LoginInput.Email);
            ModelState.AddModelError(string.Empty, "Account is locked out.");
        }
        else
        {
            _logger.LogWarning("Login failed: bad password for {Email}", LoginInput.Email);
            ModelState.AddModelError(nameof(LoginInput.Password), "Incorrect password.");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRegisterAsync()
    {
        // hide any LoginInput errors so only RegisterInput validates
        ModelState.Remove(nameof(LoginInput.Email));
        ModelState.Remove(nameof(LoginInput.Password));

        ViewData["ShowRegister"] = true;

        if (!ModelState.IsValid)
            return Page();

        var existing = await _userManager.FindByEmailAsync(RegisterInput.Email);
        if (existing != null)
        {
            _logger.LogWarning("Registration failed: {Email} already in use", RegisterInput.Email);
            ModelState.AddModelError(nameof(RegisterInput.Email), "An account with that email already exists.");
            return Page();
        }

        var user = new IdentityUser { UserName = RegisterInput.Email, Email = RegisterInput.Email };
        var result = await _userManager.CreateAsync(user, RegisterInput.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToPage("/Index");
        }

        foreach (var err in result.Errors)
            ModelState.AddModelError(string.Empty, err.Description);

        return Page();
    }
}
