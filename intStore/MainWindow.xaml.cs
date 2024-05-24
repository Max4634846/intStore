using intStore.View;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace intStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove(); 
            }

        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            InternetStoreWindow internetStoreWindow = new InternetStoreWindow();
            internetStoreWindow.Show();
        }

        private void NextReg_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "Reg";

            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }

        private void BackAuthor_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "Author";

            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }
    }
}
