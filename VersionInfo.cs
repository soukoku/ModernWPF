using System.Reflection;

[assembly: AssemblyCompany("Yin-Chun Wang")]
[assembly: AssemblyCopyright("Copyright © Yin-Chun Wang 2014")]

[assembly: AssemblyVersion(ModernWPF._ModernWPFVersionString.Release)]
[assembly: AssemblyFileVersion(ModernWPF._ModernWPFVersionString.Build)]
[assembly: AssemblyInformationalVersion(ModernWPF._ModernWPFVersionString.Build)]

namespace ModernWPF
{
    static class _ModernWPFVersionString
    {
        // keep this same in majors releases
        public const string Release = "1.0.0.0";
        // change this for each nuget release
        public const string Build = "1.1.30";
    }
}