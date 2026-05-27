using API_CHAT.Data;
using API_CHAT.Hubs;
using Microsoft.EntityFrameworkCore;
using ShareModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.WebHost.UseUrls(
    "http://0.0.0.0:5000"
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
//app.UseRouting();
app.UseStaticFiles();
app.MapHub<CallHub>("/callHub");

app.UseAuthorization();

app.MapControllers();

app.Run();
