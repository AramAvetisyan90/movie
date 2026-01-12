using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<MovieApp.Services.MovieService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve files from external directory
var mediaSettings = builder.Configuration.GetSection("MediaSettings");
var basePath = mediaSettings["BasePath"];

if (!string.IsNullOrEmpty(basePath) && Directory.Exists(basePath))
{
    var postersPath = Path.Combine(basePath, "posters");
    var videosPath = Path.Combine(basePath, "videos");

    if (Directory.Exists(postersPath))
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(postersPath),
            RequestPath = "/posters"
        });
    }

    if (Directory.Exists(videosPath))
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(videosPath),
            RequestPath = "/videos"
        });
    }
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
