using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.IncludeFields = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
List<Product> productList = new();
string connectionString = "Data Source=localhost;Initial Catalog=equipment;Integrated Security=True";
using (SqlConnection connection = new(connectionString))
{
    connection.Open();
    string query = "SELECT * FROM inventory";
    using (SqlCommand command = new SqlCommand(query, connection))
    {
        using (SqlDataReader reader = command.ExecuteReader())
        {
            Debug.WriteLine("--------> " + reader);
            while (reader.Read()) 
            {
                 Product product = new Product();
                 product.title = reader.GetString(0);
                 product.description = reader.GetString(1);
                 product.price = reader.GetInt32(2);
                 product.id = reader.GetInt32(3);
                 productList.Add(product);
            }

        }
    }
}

app.MapGet("/", () =>
{
    return productList;
});

app.MapGet("/assets", () =>
{
    var value = 0;
    foreach (Product product in productList)
    {
        value += product.price;
    }
    return value;
}
);
app.MapGet("/{id}", (int id) =>
{
    var product = new Product();
    for (int i = 0; i < productList.Count; i++)
    {
        if (productList[i].id == id)
        {
            product = productList[i];
        }
    }
    return product;
});
app.MapPost("/product", ([FromBody] Product jsonProduct) =>
{
    using (SqlConnection connection = new SqlConnection(
           connectionString))
    {
        SqlCommand command = new SqlCommand($"INSERT INTO inventory VALUES ('{jsonProduct.title}', '{jsonProduct.description}', {jsonProduct.price})", connection);
        command.Connection.Open();
        command.ExecuteNonQuery();
    }
    return Results.Ok();
});
app.MapDelete("/{id}", (int id) =>
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        SqlCommand command = new SqlCommand($"DELETE FROM inventory WHERE id = {id}", connection);
        command.Connection.Open();
        command.ExecuteNonQuery();
    }
    return Results.Ok(id);
});

app.Run();
