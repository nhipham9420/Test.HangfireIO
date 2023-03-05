using Hangfire;
using Hangfire.Storage.SQLite;
using HangfireBasicAuthenticationFilter;
using Test.Hangfire.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHangfire(x => x.UseSqlServerStorage(@"Data Source=.\SQLEXPRESS;Initial Catalog=test.hangfire;Integrated Security=True"));
builder.Services.AddHangfire(x => x
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();


builder.Services.AddTransient<IServiceManagement, ServiceManagement>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    DashboardTitle = "Test Dashboard",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter()
        {
            Pass = "123456",
            User = "admin"
        }
    }
});
app.MapHangfireDashboard();

RecurringJob.AddOrUpdate<IServiceManagement>(x => x.SyncData(), Cron.Minutely);

app.Run();
