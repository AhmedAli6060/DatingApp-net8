namespace API.Helpers;

public class UploadFiles
{
    public static IEnumerable<IConfigurationSection> GetSectionPaths(string sectionName)
    {
        return GetConfig().GetSection(sectionName).GetChildren();
    }
    private static IConfigurationRoot GetConfig()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("uploads.json", optional: true, reloadOnChange: true);
        return builder.Build();
    }

}
