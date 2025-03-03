
using ADOLib;
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
            builder.Services.AddControllers();
            builder.Services.AddScoped<Books>();
            builder.Services.AddScoped<Orders>();
            builder.Services.AddScoped<Libraries>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins("https://v0-fork-of-library.vercel.app") // Update this for production
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseCors("FrontendPolicy");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}


