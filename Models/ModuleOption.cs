// Models/ModuleOption.cs
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

public class ModuleOption        // Models/ModuleOption.cs
{
    public string AssemblyName { get; set; }   // „Step7“
    public string EngineeringDll { get; set; }   // „Step7.Engineering.dll“
    public string VersionInfo { get; set; }   // „V19“
    public bool IsSelected { get; set; }
}
