using System;
using System.Windows;

namespace OpenessApp
{
    public static class Program
    {
        // Der Einstiegspunkt der Anwendung. Das Attribut [STAThread] ist für WPF-Anwendungen erforderlich,
        // da es die Verwendung von COM-Komponenten im Single Thread Apartment (STA) ermöglicht.
        [STAThread]
        public static void Main()
        {
            // Hier kann Initialisierungscode eingefügt werden (z. B. Logging, DPI-Einstellungen, Global Exception Handling etc.)

            // Instanziierung der WPF-Applikation, abgeleitet von PrismApplication
            // Diese Klasse übernimmt die Steuerung des gesamten Application-Lebenszyklus.
            var app = new App();

            // Kein Aufruf von InitializeComponent() erforderlich!
            // Der PrismApplication-Basiscode übernimmt dies automatisch im Rahmen des Bootstrappings.

            // Startet die Applikation und damit den UI-Dispatcher sowie die Hauptansicht (Shell),
            // die über das Shell-ViewModel und die RegisterViews-Konfiguration definiert wird.
            app.Run();
        }
    }
}
