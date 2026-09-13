using BackgroundServic;
using BackgroundServic.Middlewares;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHostedService<BackgroundWorkerService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomMiddlewares();
app.UseHttpsRedirection();

app.MapGet("/send-mail", async (HttpContext httpContext) =>
{
    var message = new MimeMessage();

    var from = new MailboxAddress("Ugo", "support@hydrogentech.com.ng");
    message.From.Add(from);

    var to = new MailboxAddress("Ugo 2", "admin@hydrogentech.com.ng");
    message.To.Add(to);
    
    message.Subject = "Hello World!";

    var bb = new BodyBuilder();
    bb.TextBody = """
                  Create an ultra-realistic smartphone selfie photo of a fictional adult Japanese clean-gal style woman lying on a soft bed, 

                  faithfully preserving the reference composition, pose, outfit silhouette, lighting, and intimate mood. Subject: A fictional 20-year-old 
                  Japanese woman with a clean and elegant gyaru-inspired appearance. She has a soft

                  - Ugo
                  """;
    var imageEntity = bb.LinkedResources.Add("cat-pic.jpg");
    imageEntity.ContentId = MimeUtils.GenerateMessageId();
    
    bb.HtmlBody = $"""
                  <p>Create an ultra-realistic smartphone selfie photo of a fictional adult <em>Japanese</em> clean-gal style woman lying on a soft bed, </p>
                  <img src="cid:{imageEntity.ContentId}" alt="Pic of a cat" />
                  <p>faithfully preserving the reference composition, pose, outfit silhouette, lighting, and intimate mood. Subject: A fictional 20-year-old 
                  Japanese woman with a clean and elegant gyaru-inspired appearance. She has a soft</p>

                  <h2>- Ugo</h2>
                  """;
    // bb.Attachments.Add("cat-pic.jpg");
    
    message.Body = bb.ToMessageBody();
    
    using var smtp = new SmtpClient();
    await smtp.ConnectAsync("localhost", 1025);
    await smtp.SendAsync(message);
    await smtp.DisconnectAsync(true);
    
    return "Mail sent";

});

app.MapGet("/exception", () =>
{
    throw new Exception("This is a test exception");
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}