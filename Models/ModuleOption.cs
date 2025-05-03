// Models/ModuleOption.cs
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace OpenessApp.Models
{
    public class ModuleOption
    {
        // Name des Moduls (z.B. "Step7")
        public string AssemblyName { get; set; }

        // Name der DLL (z.B. "Step7.Engineering.dll")
        public string EngineeringDll { get; set; }

        // Versionsinfo, hier fest vorgegeben oder über den Pfad ermittelt
        public string VersionInfo { get; set; }

        // Ob das Modul ausgewählt ist
        public bool IsSelected { get; set; }
    }
}
