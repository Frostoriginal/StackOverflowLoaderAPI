
using Microsoft.EntityFrameworkCore;
using StackOverflowLoaderAPI.Data;
using StackOverflowLoaderAPI.Repositories;
using System.Net.Http;

namespace StackOverflowLoaderAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Load enviromental variables
            string connString = Environment.GetEnvironmentVariable("connstring");
            bool properlyLoadedMinimum = int.TryParse(Environment.GetEnvironmentVariable("tagstoload"), out int loadMinimumAmountOfTags);
            if (!properlyLoadedMinimum) loadMinimumAmountOfTags = 1000; //in case TryParse fails.

            bool timemsBeforeNextCall = int.TryParse(Environment.GetEnvironmentVariable("timemsbeforenextcall"), out int msBeforeNextCall);
            if (!timemsBeforeNextCall) msBeforeNextCall = 5000; //in case TryParse fails.

            bool tagsToLoadPageSizeRead = int.TryParse(Environment.GetEnvironmentVariable("tagstoloadpagesize"), out int tagsToLoadPageSize);
            if (!tagsToLoadPageSizeRead) tagsToLoadPageSize = 100; //in case TryParse fails.
            
            bool resetDatabaseOnStartup = false;
            if (Environment.GetEnvironmentVariable("resetdbonstartup").ToLower() == "true") resetDatabaseOnStartup = true;

//            TagDataDataContext db = new TagDataDataContext(connString);

            //Prepare the database and load some tags
            Helpers.connectAndCreateDB(connString, resetDatabaseOnStartup).Wait();           
            Helpers.loadSomeTags(connString, loadMinimumAmountOfTags, tagsToLoadPageSize, msBeforeNextCall).Wait();
            
            //TagData.loadMinTags();
            


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddTagDataDataContext(connString);

            //builder.Services.AddDbContext<TagDataDataContext>(options =>
            //{
            //    options.UseSqlServer(connString);
                
            //});

            

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddHealthChecks()
              .AddDbContextCheck<TagDataDataContext>()
              // execute SELECT 1 using the specified connection string 
              .AddSqlServer(connString);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseHealthChecks(path: "/howdoyoufeel");

            app.MapControllers();

            app.Run();
        }
    }
}
