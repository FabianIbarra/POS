using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using POS.ViewModels;

namespace POS.Views
{
    public partial class POSView : Window
    {
        public POSView()
        {
            InitializeComponent();
            this.DataContext = new POSViewModel();
        }

        // Un pequeño truco de UX para que, si el usuario hace clic fuera de la caja,
        // el foco regrese al buscador, garantizando que el lector de códigos siempre funcione.
        protected override void OnActivated(System.EventArgs e)
        {
            base.OnActivated(e);
            TxtBuscador.Focus();
        }
    }
}
