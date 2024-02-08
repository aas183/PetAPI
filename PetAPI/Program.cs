using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAPI.Data;
using PetAPI.Models;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PetDatabaseContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

async Task<List<PetInformationTable>> GetPets(PetDatabaseContext db) => await db.PetInformationTable.ToListAsync();

// Pet Info Table 
app.MapGet("api/petInfo", async ([FromServices] PetDatabaseContext db) =>
{ return await db.PetInformationTable.ToListAsync(); });

app.MapGet("api/petId", async ([FromServices] PetDatabaseContext db) =>
{ return await db.PetId.ToListAsync(); });

app.MapPost("api/AddPet", async ([FromServices] PetDatabaseContext db, PetInformationTable item) =>
{
    db.PetInformationTable.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});

app.MapDelete("api/DeletePet/{id}", async (PetDatabaseContext db, string id) =>
{
    var petItem = await db.PetInformationTable.FindAsync(id);
    if (petItem == null) return Results.NotFound("Pet not Found");
    db.Remove(petItem);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});

// Pet Activity Table
app.MapGet("api/petActivity", async ([FromServices] PetDatabaseContext db) =>
{ return await db.PetActivityTable.ToListAsync(); });

app.MapPost("api/AddPetActivity", async ([FromServices] PetDatabaseContext db, PetActivityTable item) =>
{
    db.PetActivityTable.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});

app.MapDelete("api/DeletePetActivity/{id}", async (PetDatabaseContext db, string id) =>
{
    var petItem = await db.PetActivityTable.FindAsync(id);
    if (petItem == null) return Results.NotFound("Pet not Found");
    db.Remove(petItem);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});

// Locking Restriction Table
app.MapGet("api/lockRestriction", async ([FromServices] PetDatabaseContext db) =>
{ return await db.LockingRestrictionTable.ToListAsync(); });

app.MapPost("api/AddLockRestriction", async ([FromServices] PetDatabaseContext db, PetActivityTable item) =>
{
    db.PetActivityTable.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});

app.MapDelete("api/DeleteLockRestriction/{id}", async (PetDatabaseContext db, string id) =>
{
    var petItem = await db.LockingRestrictionTable.FindAsync(id);
    if (petItem == null) return Results.NotFound("Pet not Found");
    db.Remove(petItem);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});



app.Run();
