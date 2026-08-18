using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using POS.Data;
using POS.Services;

namespace POS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Manejo global de excepciones para registrar y responder ante errores inesperados
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Forzar Pesos Mexicanos (es-MX) en toda la logica de C#
            var culture = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Forzar la misma cultura en la interfaz grafica (XAML).
            // Debe aplicarse ANTES de crear la primera ventana: si se sobreescribe la metadata
            // de LanguageProperty con elementos ya creados, el texto de los campos de esa
            // primera ventana no se renderiza (bug conocido de WPF).
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

            // Crear el esquema de la base de datos de forma idempotente
            DatabaseInitializer.Inicializar();

            // Sembrar usuario administrador por defecto si la tabla esta vacia
            SeedAdmin.VerificarYSembrar();

            // Mostrar la ventana de arranque (StartupUri) al final
            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogService.RegistrarError(e.Exception);
            MessageBox.Show($"Ocurrió un error inesperado: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogService.RegistrarError(ex);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogService.RegistrarError(e.Exception);
            e.SetObserved();
        }
    }
}