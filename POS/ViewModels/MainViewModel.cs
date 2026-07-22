using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Services;

namespace POS.ViewModels
{
    /// <summary>
    /// ViewModel principal que gestiona la navegación entre los módulos del sistema
    /// y controla la visibilidad de las opciones según el rol del usuario autenticado.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject? _currentViewModel;

        [ObservableProperty]
        private string _nombreUsuario = string.Empty;

        [ObservableProperty]
        private string _rolUsuario = string.Empty;

        [ObservableProperty]
        private bool _esAdministrador;

        /// <summary>
        /// Evento que se dispara al cerrar sesión para que la vista pueda redirigir al Login.
        /// </summary>
        public event EventHandler? SesionCerrada;

        public MainViewModel()
        {
            var sesion = AuthService.SesionActual;
            if (sesion != null)
            {
                NombreUsuario = sesion.NombreCompleto ?? sesion.Username ?? string.Empty;
                RolUsuario = sesion.Rol ?? string.Empty;
                EsAdministrador = string.Equals(sesion.Rol, "Administrador", StringComparison.OrdinalIgnoreCase);
            }

            IrAPOS();
        }

        [RelayCommand]
        private void IrAPOS()
        {
            CurrentViewModel = new POSViewModel();
        }

        [RelayCommand]
        private void IrAInventario()
        {
            CurrentViewModel = new InventarioViewModel();
        }

        [RelayCommand]
        private void IrAReportes()
        {
            CurrentViewModel = new ReportesViewModel();
        }

        [RelayCommand]
        private void IrAUsuarios()
        {
            CurrentViewModel = new UsuariosViewModel();
        }

        [RelayCommand]
        private void CerrarSesion()
        {
            var result = MessageBox.Show("¿Desea cerrar la sesión actual?", "Cerrar Sesión", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                AuthService.SesionActual = null;
                SesionCerrada?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
