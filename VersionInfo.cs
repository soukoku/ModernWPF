using System.Reflection;

[assembly: AssemblyCompany("Yin-Chun Wang")]
[assembly: AssemblyCopyright("Copyright © Yin-Chun Wang 2014")]

[assembly: AssemblyVersion(ModernWPF._VersionString.Release)]
[assembly: AssemblyFileVersion(ModernWPF._VersionString.Build)]
[assembly: AssemblyInformationalVersion(ModernWPF._VersionString.Build)]

namespace ModernWPF
{
    static class _VersionString
    {
        // keep this same in majors releases
        public const string Release = "1.0.0.0";
        // change this for each nuget release
        public const string Build = "1.1.33";
    }
}