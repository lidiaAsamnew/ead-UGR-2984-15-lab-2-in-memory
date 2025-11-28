using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PizzaStore API (In-Memory)",
        Version = "v1",
        Description = "Pizza API using EF Core In-Memory Database"
    });
});

// Register EF Core In-Memory database
builder.Services.AddDbContext<PizzaDb>(options =>
    options.UseInMemoryDatabase("PizzasDb"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello In-Memory!");

// CRUD Endpoints


app.MapGet("/pizzas", async (PizzaDb db) =>
    await db.Pizzas.ToListAsync()
);


app.MapGet("/pizza/{id}", async (PizzaDb db, int id) =>
    await db.Pizzas.FindAsync(id)
);


app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});


app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatePizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza == null) return Results.NotFound();

    pizza.Name = updatePizza.Name;
    pizza.Description = updatePizza.Description;

    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza == null) return Results.NotFound();

    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();
