namespace EHRJs.Models;

public class VitalsModel
{
    public string BloodPressure { get; set; }
    public string Time { get; set; }
    public string Uid { get; set; }
    public double Systolic { get; set; }
    public double Diastolic { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    public double Spo2 { get; set; }
    public bool isError { get; set; }
    public string ErrorMessage { get; set; }
    public bool isSuccess { get; set; }
    public string SuccessMessage { get; set; }
}