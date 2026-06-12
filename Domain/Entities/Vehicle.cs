namespace Domain.Entities;

public class Vehicle : BaseAuditEntity
{
    public Guid VehicleId { get; set; }

    public int MakeId { get; set; }
    public Make Make { get; set; } = null!;

    public int ModelId { get; set; }
    public Model Model { get; set; } = null!;

    public string? VIN { get; set; }
    public string? LicensePlate { get; set; }
    public int Year { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public int? MileageAtPurchase { get; set; }
    public int? Mileage { get; set; }

    public ICollection<Maintenance> MaintenanceRecords { get; set; } = [];
}
