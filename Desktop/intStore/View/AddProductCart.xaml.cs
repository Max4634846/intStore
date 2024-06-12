using intStore.Models;
using System;
using System.Windows;
using System.Windows.Input;

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
            LoadTextAddProduct(_currentProduct);
        }

        private void LoadTextAddProduct(Product _currentProduct)
        {
            ImageProductAdd.Source = _currentProduct.ImageProduct;
            NameProduct.Text = _currentProduct.NameProduct;
            Description.Text = _currentProduct.Description;
            PriceProduct.Text = Convert.ToString($"{_currentProduct.Price:F2}₽");
        }

        //Кнопки количества товара, уменьшение и увеличение товара
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

        //Кнопка добавления, закрытия и перетаскивания окна
        private void AddProductCart_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CheckNumber.Text, out int quantity))
            {
                ProductEventArgs args = new ProductEventArgs(_currentProduct, quantity);
                ProductAddedToCart?.Invoke(this, args);
                Close();
            }
            else
            {
                MessageBox.Show("Некорректное количество товара.");
            }
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

    }
}
