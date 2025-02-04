namespace EHRJs.Models;

public class VitalsModel
{
    public required string ehrID { get; set; }
    public required double Weight { get; set; }
    public required double Height { get; set; }
    public required string BloodPressure { get; set; }
    public required double Spo2 { get; set; }
}