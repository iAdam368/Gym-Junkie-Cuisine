using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Web_App.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Configure Stripe
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
//Stripe.StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];

builder.Services.AddRazorPages();
builder.Services.AddDbContext<Web_AppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Web_AppContext") ?? throw new InvalidOperationException("Connection string 'Web_AppContext' not found.")));


builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Stores.MaxLengthForKeys = 128;
})

.AddEntityFrameworkStores<Web_AppContext>()
.AddRoles<IdentityRole>()
.AddDefaultUI()
.AddDefaultTokenProviders();


builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmins", policy => policy.RequireRole("Admin"));
});

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/MenuModify", "RequireAdmins");
    });


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
    try { 
        context.Database.EnsureCreated();
        //DbInitializer.Initialize(context);
    }
    catch (SqlException) {
        // SQL Database Exception - cannot connect to the database server (probably a VPN or connection issue)
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();



using (var scope = app.Services.CreateScope())
{

    try
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<Web_AppContext>();
        context.Database.Migrate();
        var userMgr = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
        var signInMgr = services.GetRequiredService<SignInManager<IdentityUser>>();
        IdentitySeedData.Initialize(context, userMgr, roleMgr, signInMgr).Wait();
    }
    catch (SqlException)
    {
        // SQL Database Exception - cannot connect to the database server (probably a VPN or connection issue)
    }

}

app.Run();
