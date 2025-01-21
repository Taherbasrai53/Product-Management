using Microsoft.EntityFrameworkCore;
using Product_Management.Data;
using Product_Management.Repositories.Contracts;
using Product_Management.Repositories.Implementations;
using Product_Management.Services.Contracts;
using Product_Management.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options=> 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options=>
{
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyHeader().
        AllowAnyOrigin().
        AllowAnyMethod());
});

//Entity Framework implementation

builder.Services.AddTransient<IRetailersRepository, RetailersRepository>();
builder.Services.AddTransient<IStoresRepository, StoresRepository>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();

//ADO.Net implementation

//builder.Services.AddTransient<IRetailersRepository, RetailersADORepository>();
//builder.Services.AddTransient<IStoresRepository, StoresADORepository>();
//builder.Services.AddTransient<IProductsRepository, ProductsADORepository>();

builder.Services.AddTransient<IRetailerService, RetailerService>();
builder.Services.AddTransient<IStoreService, StoreService>();
builder.Services.AddTransient<IProductService, ProductService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
