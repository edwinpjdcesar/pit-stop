namespace Data.Models;

public class Vehicle : BaseEntity<Guid>
{
    public string? VIN { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int Year { get; set; }

    public int MakeId { get; set; }
    public Make Make { get; set; } = null!;

    public int ModelId { get; set; }
    public Model Model { get; set; } = null!;

    public DateTime? DatePurchased { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public int MileageAtPurchase { get; set; }
    public int Mileage { get; set; }

    public ICollection<Maintenance> MaintenanceRecords { get; set; } = [];
}
