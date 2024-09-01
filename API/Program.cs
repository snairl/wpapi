using API.Extensions;
using Domain.Users;
using Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure the HTTP request pipeline.
builder.Services
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .ConfigureMapper()
    .ConfigureAuthentication(builder.Configuration)
    .ConfigureGraphQl()
    .AddBusinessServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapGraphQL();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//    endpoints.MapGraphQL();
//});


//app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await mongoDbContext.CreateCollectionIfNotExists<User>();
}

app.Run();
