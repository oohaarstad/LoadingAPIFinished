using Microsoft.EntityFrameworkCore;
using LoadingAPI.Data;
using LoadingAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Databases/LoadingDb.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());
});

builder.Services.AddControllers();
builder.Services.AddSignalR(); // Add SignalR service

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseStaticFiles(); // Ensure static files are served

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Comment out or remove this line
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<VoteHub>("/voteHub"); // Map the SignalR hub

app.Run();
