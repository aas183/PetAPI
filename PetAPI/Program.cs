using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAPI.Data;
using PetAPI.Models;
using System.Runtime.InteropServices;
using PetAPI.Services;
using PetAPI.Controllers;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileService>();


var keyVaultEndpoint = new Uri(builder.Configuration["VaultKey"]);
var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());

KeyVaultSecret kvs = secretClient.GetSecret("PetDb");
builder.Services.AddDbContext<PetDatabaseContext>(o => o.UseSqlServer(kvs.Value));

builder.Services.AddDbContext<PetDatabaseContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();

app.MapControllers();

async Task<List<PetInformationTable>> GetPets(PetDatabaseContext db) => await db.PetInformationTable.ToListAsync();

// Pet Info Table 
app.MapGet("api/petInfo", async ([FromServices] PetDatabaseContext db) =>
{ return await db.PetInformationTable.ToListAsync(); });

app.MapGet("api/petId", async ([FromServices] PetDatabaseContext db) =>
{ return await db.PetId.ToListAsync(); });

app.MapPost("api/addPet", async ([FromServices] PetDatabaseContext db, PetInformationTable item) =>
{
    db.PetInformationTable.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});

app.MapDelete("api/deletePet/{id}", async (PetDatabaseContext db, string id) =>
{
    var pet = await db.PetInformationTable.FindAsync(id);
    if (pet == null) return Results.NotFound("Pet not Found");
    db.Remove(pet);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});

app.MapPut("api/pet/{id}/inOut/{newId}", async (PetDatabaseContext db, string id, string newId) =>
{
    var pet = await db.PetInformationTable.FindAsync(id);
    if (pet == null) return Results.NotFound("Pet not Found");
    pet.Id = newId;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("api/pet/{id}/inOut/{inOut}", async (PetDatabaseContext db, string id, string inOut) =>
{
    var pet = await db.PetInformationTable.FindAsync(id);
    if (pet == null) return Results.NotFound("Pet not Found");
    pet.InOut = inOut;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("api/pet/{id}/name/{name}", async (PetDatabaseContext db, string id, string name) =>
{
    var pet = await db.PetInformationTable.FindAsync(id);
    if (pet == null) return Results.NotFound("Pet not Found");
    pet.Name = name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("api/pet/{id}/image/{image}", async (PetDatabaseContext db, string id, string image) =>
{
    var pet = await db.PetInformationTable.FindAsync(id);
    if (pet == null) return Results.NotFound("Pet not Found");
    pet.Image = $"https://petimagestorage.blob.core.windows.net/pet-images/{image}";
    await db.SaveChangesAsync();
    return Results.NoContent();
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

app.MapDelete("api/deletePetActivity/{id}", async (PetDatabaseContext db, string id) =>
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

app.MapPost("api/addLockRestriction", async ([FromServices] PetDatabaseContext db, LockingRestrictionTable item) =>
{
    db.LockingRestrictionTable.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});

app.MapDelete("api/deleteLockRestriction/{id}", async (PetDatabaseContext db, string id) =>
{
    var petItem = await db.LockingRestrictionTable.FindAsync(id);
    if (petItem == null) return Results.NotFound("Pet not Found");
    db.Remove(petItem);
    await db.SaveChangesAsync();
    return Results.Ok(await GetPets(db));
});



app.Run();
