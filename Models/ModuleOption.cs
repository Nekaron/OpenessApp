// Models/ModuleOption.cs
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace OpenessApp.Models
{
    public class ModuleOption : INotifyPropertyChanged
    {
        public string AssemblyName { get; }
        public string FileName { get; }
        public string FullPath { get; }
        public string VersionInfo { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public ModuleOption(string fullPath)
        {
            if (!File.Exists(fullPath))
                throw new FileNotFoundException(fullPath);

            FullPath = fullPath;
            FileName = Path.GetFileName(fullPath);
            AssemblyName = Path.GetFileNameWithoutExtension(FileName);

            var fvi = FileVersionInfo.GetVersionInfo(fullPath);
            VersionInfo = fvi.FileVersion ?? "n/a";
        }

        // INotifyPropertyChanged‑Implementierung
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
