using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartFitBook.MAUI.Data;
using SmartFitBook.MAUI.Models;

namespace SmartFitBook.MAUI.Services;

public class WorkoutService
{
    private readonly DataService _dataService;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public WorkoutService(DataService dataService, IDbContextFactory<AppDbContext> contextFactory)
    {
        _dataService = dataService;
        _contextFactory = contextFactory;
    }

    public string ExportWorkoutToJson(WorkoutPlan plan)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(plan, options);
    }

    public async Task<WorkoutPlan?> ImportWorkoutFromJsonAsync(string jsonContent)
    {
        try
        {
            var plan = JsonSerializer.Deserialize<WorkoutPlan>(jsonContent);
            if (plan != null)
            {
                foreach (var day in plan.Schedule.Values)
                {
                    foreach (var item in day)
                    {
                        var exercise = await _dataService.GetExerciseByIdAsync(item.ExerciseId);
                        if (exercise != null)
                        {
                            item.Instructions = exercise.Instructions;
                            item.GifUrl = exercise.GifUrl;
                            item.Name = exercise.Name;
                        }
                    }
                }
            }
            return plan;
        }
        catch
        {
            return null;
        }
    }

    // ذخیره برنامه در دیتابیس
    public async Task<bool> SaveWorkoutToDatabaseAsync(WorkoutPlan plan)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var entity = WorkoutPlanEntity.FromWorkoutPlan(plan);

            // بررسی وجود برنامه با همین ID
            var existing = await context.WorkoutPlans
                .FirstOrDefaultAsync(w => w.WorkoutId == plan.WorkoutId);

            if (existing != null)
            {
                // به‌روزرسانی
                existing.TraineeUsername = entity.TraineeUsername;
                existing.CreatedBy = entity.CreatedBy;
                existing.StartDate = entity.StartDate;
                existing.Weeks = entity.Weeks;
                existing.WeekDaysJson = entity.WeekDaysJson;
                existing.ScheduleJson = entity.ScheduleJson;
                existing.IsActive = entity.IsActive;
            }
            else
            {
                // اضافه کردن جدید
                await context.WorkoutPlans.AddAsync(entity);
            }

            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
            return false;
        }
    }

    // دریافت برنامه‌های یک شاگرد
    public async Task<List<WorkoutPlan>> GetWorkoutsByTraineeAsync(string traineeUsername, bool onlyActive = true)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var query = context.WorkoutPlans
                .Where(w => w.TraineeUsername == traineeUsername);

            if (onlyActive)
                query = query.Where(w => w.IsActive);

            var entities = await query
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return entities.Select(e => e.ToWorkoutPlan()).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}");
            return new List<WorkoutPlan>();
        }
    }

    // دریافت یک برنامه با ID
    public async Task<WorkoutPlan?> GetWorkoutByIdAsync(string workoutId)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.WorkoutPlans
                .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

            return entity?.ToWorkoutPlan();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}");
            return null;
        }
    }

    // غیرفعال کردن برنامه (وقتی برنامه جدید ایمپورت شد)
    public async Task DeactivateWorkoutAsync(string workoutId)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.WorkoutPlans
                .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

            if (entity != null)
            {
                entity.IsActive = false;
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Deactivate error: {ex.Message}");
        }
    }

    public WorkoutPlan CreateEmptyPlan(string traineeUsername, string createdBy, int weeks, List<string> weekDays)
    {
        var schedule = new Dictionary<string, List<WorkoutDayItem>>();
        foreach (var day in weekDays)
        {
            schedule[day] = new List<WorkoutDayItem>();
        }

        return new WorkoutPlan
        {
            WorkoutId = Guid.NewGuid().ToString(),
            TraineeUsername = traineeUsername,
            CreatedBy = createdBy,
            StartDate = DateTime.Today,
            Weeks = weeks,
            WeekDays = weekDays,
            Schedule = schedule,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void AddExerciseToDay(WorkoutPlan plan, string day, WorkoutDayItem item)
    {
        if (!plan.Schedule.ContainsKey(day))
            plan.Schedule[day] = new List<WorkoutDayItem>();

        plan.Schedule[day].Add(item);
    }

    public void RemoveExerciseFromDay(WorkoutPlan plan, string day, int index)
    {
        if (plan.Schedule.ContainsKey(day) && index < plan.Schedule[day].Count)
            plan.Schedule[day].RemoveAt(index);
    }
    public async Task<bool> DeleteWorkoutAsync(string workoutId)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.WorkoutPlans
                .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

            if (entity != null)
            {
                context.WorkoutPlans.Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete error: {ex.Message}");
            return false;
        }
    }
    public async Task<List<WorkoutPlan>> GetWorkoutsByDateRangeAsync(string traineeUsername, DateTime startDate, DateTime endDate)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var entities = await context.WorkoutPlans
                .Where(w => w.TraineeUsername == traineeUsername &&
                            w.StartDate >= startDate &&
                            w.StartDate <= endDate)
                .OrderByDescending(w => w.StartDate)
                .ToListAsync();

            return entities.Select(e => e.ToWorkoutPlan()).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}");
            return new List<WorkoutPlan>();
        }
    }
}