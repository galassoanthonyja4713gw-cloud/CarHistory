using CarHistory.Api.Entities;
using CarHistory.Api.Models;

namespace CarHistory.Api.Mapping;

public static class CarEntryMapping
{
    public static CarEntryDto ToDto(this CarEntry e)
    {
        int ownershipDays = (int)((e.SoldDate ?? DateOnly.FromDateTime(DateTime.UtcNow)).ToDateTime(TimeOnly.MinValue) - e.PurchaseDate.ToDateTime(TimeOnly.MinValue)).TotalDays;
        int? milesDriven = e.OdometerAtSale.HasValue ? (e.OdometerAtSale.Value - e.OdometerAtPurchase) : null;

        return new CarEntryDto(
            e.Id,
            e.Make,
            e.Model,
            e.Trim,
            e.Year,
            e.Color,
            e.Hp,
            e.Tq,
            e.Vin,
            e.Notes,
            e.PurchaseDate,
            e.SoldDate,
            e.OdometerAtPurchase,
            e.OdometerAtSale,
            milesDriven,
            ownershipDays < 0 ? 0 : ownershipDays,
            e.CreatedUtc,
            e.UpdatedUtc
        );
    }
}
