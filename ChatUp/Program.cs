using ChatUp.Data.Database;
using ChatUp.Data.Entities;
using ChatUp.Data.Repositories;
using ChatUp.Services.Authorization;
using ChatUp.Services.Relations;

namespace ChatUp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);



        // Register DbContext with SQLite
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();


        builder.Services.AddScoped<IRepository<User>,Repository<User>>();
        builder.Services.AddScoped<IRepository<Relation>,Repository<Relation>>();
        builder.Services.AddScoped<IRepository<Message>,Repository<Message>>();
        builder.Services.AddScoped<IRepository<Session>,Repository<Session>>();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IRelationsService, RelationsService>();


        var app = builder.Build();


        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }


        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();



        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        }


        app.Run();
    }
}
