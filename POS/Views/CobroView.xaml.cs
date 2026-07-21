using System.Windows;
using POS.ViewModels;

namespace POS.Views
{
    /// <summary>
    /// Ventana modal para capturar el efectivo recibido y calcular el cambio.
    /// Se abre desde el POSViewModel al presionar Cobrar.
    /// </summary>
    public partial class CobroView : Window
    {
        public CobroView(CobroViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SolicitarCierre += (_, _) => this.Close();
        }
    }
}
