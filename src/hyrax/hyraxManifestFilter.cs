using Umbraco.Cms.Core.Manifest;

namespace hyrax.Umbraco
{
    internal class HyraxManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            var assembly = typeof(HyraxManifestFilter).Assembly;

            manifests.Add(new PackageManifest
            {
                PackageName = "hyrax",
                Version = assembly.GetName()?.Version?.ToString(3) ?? "0.1.0",
                AllowPackageTelemetry = true,
                Scripts = new string[] {
                    // List any Script files
                    // Urls should start '/App_Plugins/hyrax/' not '/wwwroot/hyrax/', e.g.
                    // "/App_Plugins/hyrax/Scripts/scripts.js"
                },
                Stylesheets = new string[]
                {
                    // List any Stylesheet files
                    // Urls should start '/App_Plugins/hyrax/' not '/wwwroot/hyrax/', e.g.
                    // "/App_Plugins/hyrax/Styles/styles.css"
                }
            });
        }
    }
}
