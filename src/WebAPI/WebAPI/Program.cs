
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
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsProfile",
                policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod();
                });
            })

            var app = builder.Build();

            app.UseCors("CorsProfile");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            

            app.MapGet("/Login", (HttpContext httpContext) =>
            {
                return "Logged in";
            })
            .WithName("Login")
            .WithOpenApi();

            app.Run();
        }
    }
}

