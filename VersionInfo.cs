using System.Reflection;

[assembly: AssemblyCompany("Yin-Chun Wang")]
[assembly: AssemblyCopyright("Copyright © Yin-Chun Wang 2013")]

[assembly: AssemblyVersion(_VersionString.Release)]
[assembly: AssemblyFileVersion(_VersionString.Build)]
[assembly: AssemblyInformationalVersion(_VersionString.Build)]

static class _VersionString
{
    // keep this same in majors releases
    public const string Release = "1.0.0.0";
    // change this for each nuget release
    public const string Build = "1.1.19";
}
