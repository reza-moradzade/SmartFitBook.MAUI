using SmartFitBook.MAUI.Models;

namespace SmartFitBook.MAUI.Services;

public class WorkoutStateService
{
    public string? TraineeUsername { get; set; }
    public int Weeks { get; set; }
    public List<string> WeekDays { get; set; } = new();
    public WorkoutPlan? CurrentPlan { get; set; }

    public void Clear()
    {
        TraineeUsername = null;
        Weeks = 0;
        WeekDays.Clear();
        CurrentPlan = null;
    }
}