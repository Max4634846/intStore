using intStore.Data;
using intStore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace intStore.View
{
    /// <summary>
    /// Interaction logic for WinBeforeRegOrder.xaml
    /// </summary>
    public partial class WinBeforeRegOrder : Window
    {
        private Customers loggedInCustomer;
        public WinBeforeRegOrder(Customers customer, InternetStoreWindow internetStoreWindow)
        {
            InitializeComponent();
            loggedInCustomer = customer;
            PriceProductOrder(internetStoreWindow.cartItemsWithImages);
            ItemSource();
            LoadText(customer);
        }
        private void LoadText(Customers customers)
        {
            PaymentMethod.SelectedValue = customers.id_Payments;
        }

        private void ItemSource()
        {
            using (var context = new InternetStoreEntities1())
            {
                var paymentsComboBox = context.Payments
                    .Select(el => new PaymentModel { id_Payments = el.id_Payments, MethodName = el.MethodName })
                    .ToList();

                PaymentMethod.ItemsSource = paymentsComboBox;
                PaymentMethod.DisplayMemberPath = "MethodName";
                PaymentMethod.SelectedValuePath = "id_Payments";
            }
        }

        private void PriceProductOrder(ObservableCollection<Product> cartItemsWithImages)
        {
            decimal totalPrice = Convert.ToDecimal(cartItemsWithImages.Sum(item => item.Price * item.Quantity));
            PriceProduct.Text = $"₽{totalPrice:F2}";
        }


        private void OKBTN_Click(object sender, RoutedEventArgs e)
        {
            int customerId = loggedInCustomer.id_Customers;

            using (var context = new InternetStoreEntities1())
            {

                var cust = context.Customers.FirstOrDefault(c => c.id_Customers == customerId);
                if (cust != null)
                {
                    if (PaymentMethod.SelectedValue == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите способ оплаты.");
                        return;
                    }

                    cust.id_Payments = (int)PaymentMethod.SelectedValue;

                    context.SaveChanges();

                    MessageBox.Show("Данные были обработаны");
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка, оплата не прошла");
                }
            }
        }
    }
}
