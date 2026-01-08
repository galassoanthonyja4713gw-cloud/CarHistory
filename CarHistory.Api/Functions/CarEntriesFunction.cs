using System.Net;
using System.Text.Json;
using CarHistory.Api.Data;
using CarHistory.Api.Entities;
using CarHistory.Api.Mapping;
using CarHistory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace CarHistory.Api.Functions;

public class CarEntriesFunction
{
    private readonly AppDbContext _db;

    public CarEntriesFunction(AppDbContext db) => _db = db;

    [Function("GetCarEntries")]
    public async Task<HttpResponseData> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "car-entries")] HttpRequestData req)
    {
        var items = await _db.CarEntries
            .AsNoTracking()
            .OrderByDescending(x => x.PurchaseDate)
            .Select(x => x.ToDto())
            .ToListAsync();

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(items);
        return res;
    }

    [Function("GetCarEntryById")]
    public async Task<HttpResponseData> GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "car-entries/{id:guid}")] HttpRequestData req,
        Guid id)
    {
        var entity = await _db.CarEntries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(entity.ToDto());
        return res;
    }

    [Function("CreateCarEntry")]
    public async Task<HttpResponseData> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "car-entries")] HttpRequestData req)
    {
        var body = await req.ReadFromJsonAsync<CreateCarEntryRequest>();
        if (body is null)
            return await BadRequestAsync(req, "Invalid JSON body.");

        var validationError = Validate(body);
        if (validationError is not null)
            return await BadRequestAsync(req, validationError);

        var entity = new CarEntry
        {
            Make = body.Make.Trim(),
            Model = body.Model.Trim(),
            Trim = body.Trim?.Trim(),
            Year = body.Year,
            Color = body.Color?.Trim(),
            Hp = body.Hp,
            Tq = body.Tq,
            Vin = body.Vin?.Trim(),
            Notes = body.Notes?.Trim(),
            PurchaseDate = body.PurchaseDate,
            SoldDate = body.SoldDate,
            OdometerAtPurchase = body.OdometerAtPurchase,
            OdometerAtSale = body.OdometerAtSale,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _db.CarEntries.Add(entity);
        await _db.SaveChangesAsync();

        var res = req.CreateResponse(HttpStatusCode.Created);
        res.Headers.Add("Location", $"/api/car-entries/{entity.Id}");
        await res.WriteAsJsonAsync(entity.ToDto());
        return res;
    }

    [Function("UpdateCarEntry")]
    public async Task<HttpResponseData> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "car-entries/{id:guid}")] HttpRequestData req,
        Guid id)
    {
        var body = await req.ReadFromJsonAsync<UpdateCarEntryRequest>();
        if (body is null)
            return await BadRequestAsync(req, "Invalid JSON body.");

        var validationError = Validate(body);
        if (validationError is not null)
            return await BadRequestAsync(req, validationError);

        var entity = await _db.CarEntries.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        entity.Make = body.Make.Trim();
        entity.Model = body.Model.Trim();
        entity.Trim = body.Trim?.Trim();
        entity.Year = body.Year;
        entity.Color = body.Color?.Trim();
        entity.Hp = body.Hp;
        entity.Tq = body.Tq;
        entity.Vin = body.Vin?.Trim();
        entity.Notes = body.Notes?.Trim();
        entity.PurchaseDate = body.PurchaseDate;
        entity.SoldDate = body.SoldDate;
        entity.OdometerAtPurchase = body.OdometerAtPurchase;
        entity.OdometerAtSale = body.OdometerAtSale;
        entity.UpdatedUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(entity.ToDto());
        return res;
    }

    [Function("DeleteCarEntry")]
    public async Task<HttpResponseData> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "car-entries/{id:guid}")] HttpRequestData req,
        Guid id)
    {
        var entity = await _db.CarEntries.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        _db.CarEntries.Remove(entity);
        await _db.SaveChangesAsync();

        return req.CreateResponse(HttpStatusCode.NoContent);
    }

    private static async Task<HttpResponseData> BadRequestAsync(HttpRequestData req, string message)
    {
        var res = req.CreateResponse(HttpStatusCode.BadRequest);
        await res.WriteStringAsync(message);
        return res;
    }

    private static string? Validate(CreateCarEntryRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Make)) return "Make is required.";
        if (string.IsNullOrWhiteSpace(r.Model)) return "Model is required.";
        if (r.Year < 1900 || r.Year > DateTime.UtcNow.Year + 1) return "Year is out of range.";
        if (r.OdometerAtPurchase < 0) return "OdometerAtPurchase must be >= 0.";
        if (r.OdometerAtSale is < 0) return "OdometerAtSale must be >= 0.";
        if (r.SoldDate.HasValue && r.SoldDate.Value < r.PurchaseDate) return "SoldDate cannot be before PurchaseDate.";
        if (r.OdometerAtSale.HasValue && r.OdometerAtSale.Value < r.OdometerAtPurchase) return "OdometerAtSale cannot be less than OdometerAtPurchase.";
        if (!string.IsNullOrWhiteSpace(r.Vin) && r.Vin.Trim().Length is < 11 or > 17) return "VIN looks invalid (expected 11–17 chars).";
        return null;
    }
}
