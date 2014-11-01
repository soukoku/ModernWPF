using System.Reflection;

[assembly: AssemblyCompany("Yin-Chun Wang")]
[assembly: AssemblyCopyright("Copyright © Yin-Chun Wang 2014")]

[assembly: AssemblyVersion(ModernWPF.Mvvm.Version.Release)]
[assembly: AssemblyFileVersion(ModernWPF.Mvvm.Version.Build)]
[assembly: AssemblyInformationalVersion(ModernWPF.Mvvm.Version.Build)]

namespace ModernWPF.Mvvm
{
    static class Version
    {
        // change this only for major releases
        public const string Release = "0.7.0.0";
        // change this for each nuget release
        public const string Build = "0.7.1";
    }
}