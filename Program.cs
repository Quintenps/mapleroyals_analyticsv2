using MapleRoyalsPlayerCount;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (context, services) =>
        {
            var appConfig = context.Configuration.GetSection("App").Get<AppConfig>();
            if (string.IsNullOrEmpty(appConfig.ServiceUrl) || 
                string.IsNullOrEmpty(appConfig.Cron) || 
                string.IsNullOrEmpty(appConfig.UserAgent) ||
                string.IsNullOrEmpty(appConfig.Endpoint))
            {
                throw new ArgumentNullException(nameof(AppConfig), "Configuration not set");
            }
            Console.WriteLine(appConfig.DisplayConfig());
            
            services.AddSingleton(appConfig);
            services.AddHostedService<Worker>();
            services.AddHttpClient<MapleRoyalsService>(c =>
            {
                c.BaseAddress = new Uri(appConfig.ServiceUrl);
                c.DefaultRequestHeaders.Add("User-Agent", appConfig.UserAgent);
            });
        }
        )
    .Build();

if (!File.Exists(DataContext.DbFile))
{
    File.Create(DataContext.DbFile);
}

await DataContext.InitializeDatabase();

await host.RunAsync();