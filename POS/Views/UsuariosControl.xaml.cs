using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using POS.Helpers;
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

        /// <summary>
        /// Bloquea la escritura de caracteres no permitidos en el nombre de usuario.
        /// Solo se permiten letras, números, puntos y guiones.
        /// </summary>
        private void UsernameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!UsuarioInputHelper.EsEntradaValida(e.Text))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Bloquea el pegado de texto con caracteres no permitidos en el nombre de usuario.
        /// </summary>
        private void UsernameTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var texto = (string)e.DataObject.GetData(typeof(string));
                if (string.IsNullOrEmpty(texto) || !UsuarioInputHelper.EsEntradaValida(texto))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Sanea el texto del campo de usuario por si algún carácter no permitido logra ingresarse.
        /// </summary>
        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var textoLimpio = UsuarioInputHelper.Sanitizar(textBox.Text);
                if (textBox.Text != textoLimpio)
                {
                    textBox.Text = textoLimpio;
                    textBox.CaretIndex = textoLimpio.Length;
                }
            }
        }
    }
}
