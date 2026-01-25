namespace BilkoNavigator_.Data.Seed
{
    using System.Text.Json;
    using BilkoNavigator_.Models;

    public static class HerbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IWebHostEnvironment env)
        {
            if (context.Herbs.Any())
                return; // вече има данни

            var filePath = Path.Combine(env.ContentRootPath, "Data", "herbs.json");

            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath);

            var herbs = JsonSerializer.Deserialize<List<Herb>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (herbs == null) return;

            //context.Herbs.AddRange(herbs);
            //await context.SaveChangesAsync();
            foreach (var herb in herbs)
            {
                if (!context.Herbs.Any(h => h.PopularName == herb.PopularName))
                {
                    context.Herbs.Add(herb);
                }
            }

            await context.SaveChangesAsync();
        }
    }

}
