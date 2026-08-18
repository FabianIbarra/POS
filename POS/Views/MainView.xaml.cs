using System.Windows;
using POS.ViewModels;

namespace POS.Views
{
    /// <summary>
    /// Ventana principal del sistema que contiene el menú de navegación lateral
    /// y el área de contenido que cambia según el módulo activo.
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            this.DataContext = viewModel;

            viewModel.SesionCerrada += ViewModel_SesionCerrada;
        }

        private void ViewModel_SesionCerrada(object? sender, System.EventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
            this.Close();
        }
    }
}
