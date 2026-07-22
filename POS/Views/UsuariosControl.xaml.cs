using System.Windows;
using System.Windows.Controls;
using POS.ViewModels;

namespace POS.Views
{
    /// <summary>
    /// Control de usuario que encapsula la interfaz de Gestion de Usuarios.
    /// Se utiliza dentro del MainView para la navegacion por modulos.
    /// </summary>
    public partial class UsuariosControl : UserControl
    {
        public UsuariosControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sincroniza la contraseña del PasswordBox con el ViewModel.
        /// Excepcion MVVM necesaria por limitaciones de binding en WPF.
        /// </summary>
        private void PasswordBoxContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UsuariosViewModel viewModel)
            {
                viewModel.Contrasena = PasswordBoxContrasena.Password;
            }
        }
    }
}
