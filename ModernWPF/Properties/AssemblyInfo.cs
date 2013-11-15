﻿using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ModernWPF")]
[assembly: AssemblyDescription("A UI lib for creating modern apps in WPF 4.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("ModernWPF")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("ModernWPF.Tests")]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.MainAssembly)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("http://modernwpf", "ModernWPF")]
[assembly: XmlnsDefinition("http://modernwpf", "ModernWPF.Controls")]
[assembly: XmlnsDefinition("http://modernwpf", "ModernWPF.Converters")]
[assembly: XmlnsDefinition("http://modernwpf", "ModernWPF.Resources")]
