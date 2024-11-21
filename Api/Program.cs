

using minimal_api_desafio;

IHostBuilder CreateHostBuilder(string[] args){
  return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => {
    webBuilder.UseStartup<Startup>();
  });
}

CreateHostBuilder(args).Build().Run();