var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IMcFitCourseApi, McFitCourseApi>();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();
app.Run();