using Microsoft.EntityFrameworkCore;
using SmartFitBook.MAUI.Data;
using SmartFitBook.MAUI.Models;

namespace SmartFitBook.MAUI.Services;

public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private User? _currentUser;

    public AuthService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;
    public bool IsTrainer => _currentUser?.Role == "Trainer";
    public bool IsTrainee => _currentUser?.Role == "Trainee";

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                _currentUser = user;
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            return false;
        }
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public async Task<bool> CreateTraineeAsync(string username, string password, string trainerUsername)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();

            // بررسی عدم تکراری بودن یوزرنیم
            var exists = await context.Users.AnyAsync(u => u.Username == username);
            if (exists) return false;

            var trainee = new User
            {
                Username = username,
                Password = password,
                Role = "Trainee",
                TrainerUsername = trainerUsername,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(trainee);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Create trainee error: {ex.Message}");
            return false;
        }
    }

    public async Task<List<User>> GetMyTraineesAsync()
    {
        if (!IsTrainer) return new List<User>();

        try
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .Where(u => u.Role == "Trainee" && u.TrainerUsername == _currentUser!.Username)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get trainees error: {ex.Message}");
            return new List<User>();
        }
    }
}