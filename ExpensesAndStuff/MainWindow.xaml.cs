using ExpensesAndStuff.ViewModels;
using System.Windows;

namespace ExpensesAndStuff
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainVM)
        {
            InitializeComponent();
            DataContext = mainVM;
        }
    }
}