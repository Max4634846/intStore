using intStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace intStore.View
{
    /// <summary>
    /// Interaction logic for AddProductCart.xaml
    /// </summary>
    public partial class AddProductCart : Window
    {
        private Product _currentProduct;
        public event EventHandler<ProductEventArgs> ProductAddedToCart;
        public AddProductCart(Product currentProduct)
        {
            InitializeComponent();
            _currentProduct = currentProduct;
            ImageProductAdd.Source = _currentProduct.ImageProduct;
            NameProduct.Text = _currentProduct.NameProduct;
            Description.Text = _currentProduct.Description;
            NutritionalValueProduct.Text = _currentProduct.NutritionalValue;
        }
        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CheckNumber.Text, out int currentQuantity))
            {
                currentQuantity++;
                CheckNumber.Text = currentQuantity.ToString();
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CheckNumber.Text, out int currentQuantity))
            {
                if (currentQuantity > 1)
                {
                    currentQuantity--;
                    CheckNumber.Text = currentQuantity.ToString();
                }
            }
        }

        private void AddProductCart_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CheckNumber.Text, out int quantity))
            {
                // Создаем аргументы события
                ProductEventArgs args = new ProductEventArgs(_currentProduct, quantity);
                // Вызываем событие
                ProductAddedToCart?.Invoke(this, args);
                // Закрываем окно
                Close();
            }
            else
            {
                MessageBox.Show("Некорректное количество товара.");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}
