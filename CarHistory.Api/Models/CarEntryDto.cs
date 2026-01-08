namespace CarHistory.Api.Models;

public record CarEntryDto(
    Guid Id,
    string Make,
    string Model,
    string? Trim,
    int Year,
    string? Color,
    int? Hp,
    int? Tq,
    string? Vin,
    string? Notes,
    DateOnly PurchaseDate,
    DateOnly? SoldDate,
    int OdometerAtPurchase,
    int? OdometerAtSale,
    int? MilesDriven,
    int OwnershipDays,
    DateTime CreatedUtc,
    DateTime UpdatedUtc
);
