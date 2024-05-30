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
using System.Data.SqlClient;


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


        public InternetStoreWindow(Customers customers)
        {
            InitializeComponent();
            LoadProductList();
            loggedInCustomer = customers;
            LoadCartItemsForUser(loggedInCustomer.id_Customers);
            UpdateCartButton();
        }

        private void UpdateCartButton()
        {
            int itemsInCartCount;
            using (var context = new InternetStoreEntities1())
            {
                itemsInCartCount = context.Cart.Count(item => item.id_Customer == loggedInCustomer.id_Customers);
            }

            btnCart.Text = itemsInCartCount.ToString();
        }
        private void LoadCartItemsForUser(int customerId)
        {

            ImageManipulation imageManipulation = new ImageManipulation();
            using (var context = new InternetStoreEntities1())
            {
                var cartItemsForUser = context.Cart
                    .Where(item => item.id_Customer == customerId)
                    .Select(item => new
                    {
                        IdProduct = item.OrdersWithCart.id_OrderWithCart,
                        ImageProductData = item.OrdersWithCart.Goods.ImageProduct,
                        NameProduct = item.OrdersWithCart.Goods.NameProduct,
                        Quantity = item.Quantity,
                    })
                    .ToList();

                var cartItemsWithImages = cartItemsForUser
                    .Select(item => new Product
                    {
                        id_Product = item.IdProduct,
                        ImageProduct = imageManipulation.GetPhotoFromDataBase(item.ImageProductData),
                        NameProduct = item.NameProduct,
                        Quantity = item.Quantity,
                    })
                    .ToList();

                CartList.ItemsSource = cartItemsWithImages;
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
                    NutritionalValue = v.NutritionalValue,
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


        private void AddProductToCartHandler(object sender, ProductEventArgs e)
        {
            AddProductToCart(loggedInCustomer.id_Customers, e.Product.id_Product, e.Quantity);
            MessageBox.Show("Товар был добавлен в корзину!");

            LoadCartItemsForUser(loggedInCustomer.id_Customers);
            UpdateCartButton();
        }
        private void AddProductToCart(int customerId, int productId, int quantity)
        {
            using (var context = new InternetStoreEntities1())
            {
                context.Database.ExecuteSqlCommand("EXEC AddProductToCart @CustomerId, @ProductId, @Quantity",
                                                    new SqlParameter("@CustomerId", customerId),
                                                    new SqlParameter("@ProductId", productId),
                                                    new SqlParameter("@Quantity", quantity));
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
                    addProductCart.ProductAddedToCart += AddProductToCartHandler;
                    addProductCart.Show();
                }
                else
                {
                    MessageBox.Show("Продукт не выбран.");
                }
            }

        }


        private void DeleteProductFromCart(int customerId, int productId)
        {
            using (var context = new InternetStoreEntities1())
            {
                context.Database.ExecuteSqlCommand("EXEC DeleteProductFromCart @CustomerId, @ProductId",
                                                    new SqlParameter("@CustomerId", customerId),
                                                    new SqlParameter("@ProductId", productId));
            }
        }

        private void BtnDeleteProductCart_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Product productToDelete = button.CommandParameter as Product;
                if (productToDelete != null)
                {
                    DeleteProductFromCart(loggedInCustomer.id_Customers, productToDelete.id_Product);
                    MessageBox.Show("Товар был удален из корзины!");

                    LoadCartItemsForUser(loggedInCustomer.id_Customers);
                    UpdateCartButton();
                }
            }
        } 

        private void BtnCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            //...
        }

    }
}
