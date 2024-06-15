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
        /// <summary>
        /// /// Обработчик клика по кнопке "Добавить товар в корзину".
        /// Пытается преобразовать текст из поля CheckNumber в число для указания количества товара.
        /// Если преобразование успешно, создает аргументы события ProductEventArgs с текущим продуктом и указанным количеством.
        /// Затем вызывает событие ProductAddedToCart, уведомляя подписанные на него методы о добавлении товара в корзину,
        /// и закрывает окно добавления товара. В случае неудачи при преобразовании отображает сообщение об ошибке.
        /// </summary>
        /// <param name="sender">Объект, инициировавший событие (кнопка).</param>
        /// <param name="e">Аргументы события.</param>
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
