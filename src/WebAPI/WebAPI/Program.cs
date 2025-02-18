using LibLibrary.Services;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

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

            app.UseAuthorization();

            

            app.MapPost("/Login", (string email, string password) =>
            {
                if (LibUser.Login(email, password))
                {
                    var user = LibUser.GetUserByEmail(email);
                    return Results.Ok(new { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email });
                }
                return Results.BadRequest( "Error Logging in");
             })
            .WithName("Login")
            .WithOpenApi();

            app.Run();
        }
    }
}

