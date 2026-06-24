using System.Text.Json;

namespace SmartFitBook.MAUI.Models;

public class WorkoutPlanEntity
{
    public int Id { get; set; }
    public string WorkoutId { get; set; } = string.Empty;
    public string TraineeUsername { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int Weeks { get; set; }
    public string WeekDaysJson { get; set; } = "[]";
    public string ScheduleJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    // تبدیل به WorkoutPlan
    public WorkoutPlan ToWorkoutPlan()
    {
        return new WorkoutPlan
        {
            WorkoutId = WorkoutId,
            TraineeUsername = TraineeUsername,
            CreatedBy = CreatedBy,
            StartDate = StartDate,
            Weeks = Weeks,
            WeekDays = JsonSerializer.Deserialize<List<string>>(WeekDaysJson) ?? new(),
            Schedule = JsonSerializer.Deserialize<Dictionary<string, List<WorkoutDayItem>>>(ScheduleJson) ?? new(),
            CreatedAt = CreatedAt,
            IsActive = IsActive
        };
    }

    // ساخت از WorkoutPlan
    public static WorkoutPlanEntity FromWorkoutPlan(WorkoutPlan plan)
    {
        return new WorkoutPlanEntity
        {
            WorkoutId = plan.WorkoutId,
            TraineeUsername = plan.TraineeUsername,
            CreatedBy = plan.CreatedBy,
            StartDate = plan.StartDate,
            Weeks = plan.Weeks,
            WeekDaysJson = JsonSerializer.Serialize(plan.WeekDays),
            ScheduleJson = JsonSerializer.Serialize(plan.Schedule),
            CreatedAt = plan.CreatedAt,
            IsActive = plan.IsActive
        };
    }
}