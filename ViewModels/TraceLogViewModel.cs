// Ermöglicht PropertyBinding mit automatischer UI-Aktualisierung
using Prism.Mvvm;

// Zugriff auf den zentralen Log-Service
using OpenessApp.Services;

// Zugriff auf das Modell für eine einzelne Logzeile (mit Typ + Nachricht)
using OpenessApp.Models;

// Zugriff auf dynamische Liste mit Benachrichtigung
using System.Collections.ObjectModel;

namespace OpenessApp.ViewModels
{
    // ViewModel für die TraceLogView – verbindet Logdaten mit dem UI
    public class TraceLogViewModel : BindableBase
    {
        // Liste aller Logzeilen – wird vom Service bereitgestellt
        public ObservableCollection<TraceLogEntry> Entries { get; }

        // Konstruktor – wird vom Prism-Container automatisch aufgerufen
        public TraceLogViewModel(TraceLogService traceLogService)
        {
            // Übergabe des zentralen Dienstes (Singleton)
            Entries = traceLogService.Entries;
        }
    }
}
