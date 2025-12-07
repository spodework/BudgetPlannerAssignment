using ExpensesAndStuff.ViewModels;
using System.Windows;

namespace ExpensesAndStuff
{
    public partial class MainWindow : Window
    {

        public MainWindow(MainViewModel mainVM)
        {
            InitializeComponent();
            DataContext = mainVM;  // Set the DataContext to the injected ViewModel            
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //await viewModel.LoadAsync();

        }
    }
}