using Microsoft.EntityFrameworkCore;
using SmartFitBook.MAUI.Models;

namespace SmartFitBook.MAUI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<WorkoutPlanEntity> WorkoutPlans { get; set; }  // اضافه شده

    public static string DbPath =>
        Path.Combine(FileSystem.AppDataDirectory, "workoutapp.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            Password = "admin123",
            Role = "Trainer",
            //Username = "A",
            //Password = "123",
            //Role = "Trainee",
            TrainerUsername = null,
            CreatedAt = DateTime.UtcNow
        });

        // تنظیمات جدول WorkoutPlans
        modelBuilder.Entity<WorkoutPlanEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.WorkoutId).IsUnique();
            entity.HasIndex(e => e.TraineeUsername);
        });
    }
}