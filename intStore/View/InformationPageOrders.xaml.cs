using intStore.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using intStore.Data;


namespace intStore.View
{
    /// <summary>
    /// Interaction logic for InformationPageOrders.xaml
    /// </summary>
    public partial class InformationPageOrders : Window
    {
        public InformationPageOrders(Customers customer, InternetStoreWindow internetStoreWindow)
        {
            InitializeComponent();
            LoadText(customer);
            PriceProductOrder(internetStoreWindow.cartItemsWithImages);
        }

        private void LoadText(Customers customer)
        {
            UserName.Text = $"{customer.Name}";
            Phone.Text = $"{customer.Phone}";
        }

        private void PriceProductOrder(ObservableCollection<Product> cartItemsWithImages)
        {
            decimal totalPrice = Convert.ToDecimal(cartItemsWithImages.Sum(item => item.Price * item.Quantity));
            PriceProduct.Text = $"₽{totalPrice:F2}";
        }


        private void OKBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
