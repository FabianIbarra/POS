using System.Windows;
using POS.ViewModels;

namespace POS.Views
{
    /// <summary>
    /// Vista del formulario de login. Sigue la arquitectura MVVM estricta: 
    /// cero lógica de negocio en el code-behind.
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            // Asignar el ViewModel al DataContext
            var viewModel = new LoginViewModel();
            this.DataContext = viewModel;

            // Suscribirse a los eventos del ViewModel para navegar tras login exitoso
            viewModel.LoginExitoso += ViewModel_LoginExitoso!;
            viewModel.LoginFallido += ViewModel_LoginFallido!;

            // Configurar el binding de la contraseña
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        /// <summary>
        /// Maneja la contraseña del PasswordBox y la vincula al ViewModel.
        /// Esto es una excepción permitida por limitaciones de binding en WPF.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Mostrar u ocultar el placeholder dependiendo si hay texto
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Collapsed;

            if (this.DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }

        /// <summary>
        /// Mantiene sincronizado el texto visible con el ViewModel cuando el usuario escribe con el ojo abierto.
        /// </summary>
        private void PasswordVisibleBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Mostrar u ocultar el placeholder dependiendo si hay texto
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordVisibleBox.Text) ? Visibility.Visible : Visibility.Collapsed;

            if (this.DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = PasswordVisibleBox.Text;
            }
        }

        private void ViewModel_LoginExitoso(object sender, System.EventArgs e)
        {
            // 1. Instanciamos la ventana principal (Punto de Venta)
            var posView = new POSView();

            // 2. La mostramos en pantalla
            posView.Show();

            // 3. Cerramos la ventana actual de Login para que no quede abierta en segundo plano
            this.Close();
        }

        private void ViewModel_LoginFallido(object sender, string mensaje)
        {
            // El mensaje ya se muestra en el ViewModel (MensajeError)
            // Esta es solo una notificación adicional si es necesaria
        }

        /// <summary>
        /// Intercambia la visibilidad entre la caja de puntos y la caja de texto normal.
        /// </summary>
        private void BtnMostrarContrasena_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                // Mostrar el texto y ocultar los puntos
                PasswordVisibleBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordVisibleBox.Visibility = Visibility.Visible;
                BtnMostrarContrasena.Content = "🔒︎"; // Cambiar icono
            }
            else
            {
                // Mostrar los puntos y ocultar el texto
                PasswordBox.Password = PasswordVisibleBox.Text;
                PasswordVisibleBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
                BtnMostrarContrasena.Content = "👁"; // Cambiar icono
            }
        }

        /// <summary>
        /// Intercepta la pulsación de teclas antes de que se escriban en la caja de texto.
        /// Si detecta un espacio, cancela la acción.
        /// </summary>
        private void Password_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                // Al marcar 'Handled' como true, le decimos a WPF que ya manejamos esta tecla
                // y que NO debe escribirla en la caja de texto/contraseña.
                e.Handled = true;
            }
        }
    }
}
