﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web_App.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<Web_AppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Web_AppContext") ?? throw new InvalidOperationException("Connection string 'Web_AppContext' not found.")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

// Creates a database for context if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<Web_AppContext>();
    context.Database.EnsureCreated();
    //DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
