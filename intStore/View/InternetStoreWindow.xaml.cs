using intStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using intStore.Utils;
using intStore.Models;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.Data.Entity;
using System.Collections.ObjectModel;


namespace intStore.View
{
    /// <summary>
    /// Interaction logic for InternetStoreWindow.xaml
    /// </summary>
    public partial class InternetStoreWindow : Window
    {
        private Product SelectedItem;
        private List<Product> productList = new List<Product>();
        private Customers loggedInCustomer;
        private List<Product> cartItems = new List<Product>();
        public InternetStoreWindow(Customers customers)
        {
            InitializeComponent();
            LoadProductList();
            loggedInCustomer = customers;
            LoadCartItemsForUser(loggedInCustomer.id_Customers);

        }
        private void LoadCartItemsForUser(int customerId)
        {
            using (var context = new InternetStoreEntities1())
            {
                // Загрузите товары корзины для указанного пользователя из базы данных
                var cartItemsForUser = context.Cart
                    .Where(item => item.id_Customer == customerId)
                    .Select(item => new Product
                    {
                        Price = item.id_OrdersWithCart,
                        // Добавьте другие свойства товара, которые хотите отобразить
                    })
                    .ToList();

                // Установите загруженные товары в качестве источника данных для ListBox
                CartListBox.ItemsSource = cartItemsForUser;
            }
        }

        private void LoadProductList()
        {
            var context = new InternetStoreEntities1();
            ImageManipulation imageManipulation = new ImageManipulation();
            var realEstatesFromDb = context.Goods.ToList();

            foreach (var v in realEstatesFromDb)
            {
                Product product = new Product
                {
                    id_Product = v.id_Product,
                    NameProduct = v.NameProduct,
                    Description = v.Description,
                    Weight = v.Weight,
                    Price = v.Price,
                    Quantity = v.Quantity,
                    DateProduct = v.DateProduct,
                    ImageProduct = imageManipulation.GetPhotoFromDataBase(v.ImageProduct),
                };

                productList.Add(product);
            }

            ItemsList.ItemsSource = productList;
        }



        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "MainPage";

            foreach( TabItem tabItem in mainTabControl.Items)
            {
                if(tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }

        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
           
            string tabName = "Product";

            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "Report";

            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }
        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "Cart";

            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                var select = button.CommandParameter as Product;
                if (select != null)
                {
                    SelectedItem = select;
                    AddProductCart addProductCart = new AddProductCart(SelectedItem);
                    // Подписываемся на событие добавления товара в корзину
                    addProductCart.ProductAddedToCart += AddProductToCartHandler;
                    addProductCart.Show();
                }
                else
                {
                    MessageBox.Show("Продукт не выбран.");
                }
            }

        }
        private void AddProductToCart(int customerId, int productId, int quantity)
        {
            using (var context = new InternetStoreEntities1())
            {
                var cart = context.Cart
                    .Include(c => c.OrdersWithCart)
                    .FirstOrDefault(c => c.id_Customer == customerId && c.OrdersWithCart.id_Product == productId);

                if (cart != null)
                {
                    cart.Quantity += quantity;
                }
                else
                {
                    var newOrderWithCart = new OrdersWithCart
                    {
                        id_Product = productId,
                        Quantity = quantity
                    };

                    context.OrdersWithCart.Add(newOrderWithCart);
                    context.SaveChanges();

                    var newCart = new Cart
                    {
                        id_Customer = customerId,
                        id_OrdersWithCart = newOrderWithCart.id_OrderWithCart,
                        Quantity = quantity
                    };

                    context.Cart.Add(newCart);
                    
                }

                context.SaveChanges();

            }
        }

        private void AddProductToCartHandler(object sender, ProductEventArgs e)
        {
            // Вызываем метод добавления товара в корзину, передавая информацию о товаре и его количестве
            AddProductToCart(loggedInCustomer.id_Customers, e.Product.id_Product, e.Quantity);
            MessageBox.Show("Product added to cart successfully!");
        }

    }
}
