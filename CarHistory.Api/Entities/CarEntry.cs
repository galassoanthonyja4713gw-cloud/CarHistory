namespace CarHistory.Api.Entities;

public class CarEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Car details
    public string Make { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string? Trim { get; set; }
    public int Year { get; set; }
    public string? Color { get; set; }
    public int? Hp { get; set; }
    public int? Tq { get; set; }
    public string? Vin { get; set; }
    public string? Notes { get; set; }

    // Ownership details
    public DateOnly PurchaseDate { get; set; }
    public DateOnly? SoldDate { get; set; }
    public int OdometerAtPurchase { get; set; }
    public int? OdometerAtSale { get; set; }

    // Audit
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
