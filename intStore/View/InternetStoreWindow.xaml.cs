using intStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using intStore.Utils;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;





namespace intStore.View
{
    /// <summary>
    /// Interaction logic for InternetStoreWindow.xaml
    /// </summary>
    public partial class InternetStoreWindow : Window
    {
        private Product SelectedItem;
        private List<Product> productList = new List<Product>();
        private List<CategoriesModel> categoriesList = new List<CategoriesModel>();
        private Customers loggedInCustomer;
        private bool IsMaximized = false;
        private List<Product> filteredProducts = new List<Product>();
        public InternetStoreWindow(Customers customers)
        {
            InitializeComponent();
            LoadProductList();
            LoadCategoriesList();
            loggedInCustomer = customers;
            LoadCartItemsForUser(loggedInCustomer.id_Customers);
            UpdateCartButton();
            RefreshPage();
            LoadedText(customers);
            ItemSource();
        }

        private void LoadedText(Customers customers)
        {
            DateReg.Text = $"{customers.RegisterDate:yyyy.MM.dd}";
            NumberPhone.Text = $"{customers.Phone}";
            Address.Text = $"{customers.Address}";
            UserName.Text = $"{customers.Name}";
            SurName.Text = $"{customers.SurName}";
            PaymentsComboBox.SelectedValuePath = Convert.ToString(customers.id_Payments);
        }

        //Обновление счётчика корзины...
        private void UpdateCartButton()
        {
            int itemsInCartCount;
            using (var context = new InternetStoreEntities1())
            {
                itemsInCartCount = context.Cart.Count(item => item.id_Customer == loggedInCustomer.id_Customers);
            }
            btnCart.Text = itemsInCartCount.ToString();
        }

        //Отображение данных в корзине... 
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
                        Price = item.OrdersWithCart.Goods.Price,
                        Status = item.Status.NameStatus,
                        

                    }).ToList();

                    var cartItemsWithImages = cartItemsForUser
                    .Select(item => new Product
                    {
                        id_Product = item.IdProduct,
                        ImageProduct = imageManipulation.GetPhotoFromDataBase(item.ImageProductData),
                        NameProduct = item.NameProduct,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Status = item.Status,
                        
                    }).ToList();
  
                CartList.ItemsSource = cartItemsWithImages;
                
                decimal totalPrice = Convert.ToDecimal(cartItemsWithImages.Sum(item => item.Price * item.Quantity));
                Cost.Text = $"₽{totalPrice:F2}";

                if(totalPrice > 1000)
                {
                    BuyProductCart.Visibility = Visibility.Visible;
                    minPrice.Visibility = Visibility.Hidden;
                }
                else
                {
                    BuyProductCart.Visibility = Visibility.Hidden;
                    minPrice.Visibility = Visibility.Visible;
                }
            }
        }

        //Списки с товарами
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
                    id_Categories = v.Categories.id_Categories,
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

        private void LoadCategoriesList()
        {
            var context = new InternetStoreEntities1();
            ImageManipulation imageManipulation = new ImageManipulation();
            var realEstatesFromDb = context.Categories.ToList();

            foreach (var v in realEstatesFromDb)
            {
                CategoriesModel categoriesProduct = new CategoriesModel
                {
                    id_Categories = v.id_Categories,
                    NameCategories = v.NameCategories,
                    Desciptions = v.Desciptions,
                    ImageCategories = imageManipulation.GetPhotoFromDataBase(v.ImageCategories),
                };
                categoriesList.Add(categoriesProduct);
            }
            CatalogList.ItemsSource = categoriesList;
        }


        //Методы для кнопок с хранимыми процедурами внутри
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
                context.Database.ExecuteSqlCommand(
                    "EXEC AddProductToCart @CustomerId, @ProductId, @Quantity",
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@Quantity", quantity));
            }
        }

        private void DeleteProductFromCart(int customerId, int productId)
        {
            using (var context = new InternetStoreEntities1())
            {
                context.Database.ExecuteSqlCommand(
                    "EXEC DeleteProductFromCart @CustomerId, @ProductId",
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@ProductId", productId));
            }
        }

        private void BuyProduct(int customerId, int cartProductId)
        {
            using (var context = new InternetStoreEntities1())
            {
                context.Database.ExecuteSqlCommand("EXEC MoveCartItemToOrder @CustomerId, @CartItemId",
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@CartItemId", cartProductId));
            }
        }

        //Добавление товара в корзину, удаление
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
                        addProductCart.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Продукт не выбран.");
                }
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
                    MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить товар?", "Удаление товара", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        DeleteProductFromCart(loggedInCustomer.id_Customers, productToDelete.id_Product);
                        MessageBox.Show("Товар был удален из корзины!", "Предупреждение об удалении товара", MessageBoxButton.OK, MessageBoxImage.Warning);

                        LoadCartItemsForUser(loggedInCustomer.id_Customers);
                        UpdateCartButton();
                    }
                    else return;
                }
            }
        }

        //Покупка товара, данные отправляется в заказы
        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            Button buttonBuy = sender as Button;
            if (buttonBuy != null)
            {
                Product productToBuy = buttonBuy.CommandParameter as Product;
                if (productToBuy != null)
                {
                        MessageBoxResult result = MessageBox.Show("Вы точно хотите приобрести данный товар?", "Покупка товара", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            BuyProduct(loggedInCustomer.id_Customers, productToBuy.id_Product);
                            MessageBox.Show("Данный товар был приобретен, ваш статус заказа: оформлен", "Покупка товара завершена", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadCartItemsForUser(loggedInCustomer.id_Customers);
                        }
                }
            }
        }

        //Выбор определенной категории с товарами 
        private void BtnCategoriesProduct_Click(object sender, RoutedEventArgs e)
        {
            txtSearchCatalog.Visibility = Visibility.Visible;
            searchCatalog.Visibility = Visibility.Visible;

            txtSearchProduct.Visibility = Visibility.Hidden;
            searchProduct.Visibility = Visibility.Hidden;

            Button button = sender as Button;
            if (button != null)
            {
                CategoriesModel selectedCategory = button.CommandParameter as CategoriesModel;
                if (selectedCategory != null)
                {
                    filteredProducts = productList.Where(product => product.id_Categories == selectedCategory.id_Categories).ToList();
                    TabItem tabProducts = mainTabControl.FindName("Product") as TabItem;

                    if (tabProducts != null)
                    {
                        tabProducts.IsSelected = true;
                        ItemsList.ItemsSource = filteredProducts;
                    }
                }
            }
        }


        //Кнопки для перехода между вкладками TabControl...
        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "MainPage";
            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }

        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {

            txtSearchProduct.Visibility = Visibility.Visible;
            searchProduct.Visibility = Visibility.Visible;

            txtSearchCatalog.Visibility = Visibility.Hidden;
            searchCatalog.Visibility = Visibility.Hidden;

            string tabName = "Product";
            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    if (ItemsList.ItemsSource != productList)
                    {

                        ItemsList.ItemsSource = productList;
                    }
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
        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "Orders";
            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }

        private void BtnPersonalAccount_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "PersonalAccount";
            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }
        private void BtnСompletedOrders_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "CompletedOrders";
            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }

        private void BtnCloseApp_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        //Сортировка товара по имени
        private void txtSearchCart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = txtSearchCart.Text.ToLower();

            ICollectionView view = CollectionViewSource.GetDefaultView(CartList.ItemsSource);
            if (view != null)
            {
                view.Filter = (obj) =>
                {
                    Product product = obj as Product;
                    if (product != null)
                    {
                        return product.NameProduct.ToString().ToLower().Contains(filterText);
                    }
                    return false;
                };
            }
        }

        private void txtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = txtSearchProduct.Text.ToLower();


            if (!string.IsNullOrWhiteSpace(filterText))
            {
                searchProduct.Visibility = Visibility.Hidden;
            }
            else
            {
                searchProduct.Visibility = Visibility.Visible;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(productList);
            if (view != null)
            {
                view.Filter = (obj) =>
                {
                    Product product = obj as Product;
                    if (product != null)
                    {
                        return product.NameProduct.ToString().ToLower().Contains(filterText);
                    }
                    return false;
                };
            }
        }


        private void txtSearchCatalog_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = txtSearchCatalog.Text.ToLower();
            
            if(!string.IsNullOrWhiteSpace(filterText))
            {
                searchCatalog.Visibility = Visibility.Hidden; 
            }
            else
            {
                searchCatalog.Visibility = Visibility.Visible;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(filteredProducts);
            if (view != null)
            {
                view.Filter = (obj) =>
                {
                    Product product = obj as Product;
                    if (product != null)
                    {
                        return product.NameProduct.ToString().ToLower().Contains(filterText);
                    }
                    return false;
                };
            }
        }

        //Масштабирование окна
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1280;
                    this.Height = 850;

                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;

                    IsMaximized = true;
                }
            }
        }
        private void ItemSource()
        {
            using (var context = new InternetStoreEntities1())
            {

                var paymentsComboBox = context.Payments.Select(el => new PaymentModel { id_Payments = el.id_Payments, MethodName = el.MethodName }).ToList();

                PaymentsComboBox.ItemsSource = paymentsComboBox;
                PaymentsComboBox.DisplayMemberPath = "MethodName";
                PaymentsComboBox.SelectedValuePath = "id_Payments";
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            int customerId = loggedInCustomer.id_Customers;

            using (var context = new InternetStoreEntities1())
            {
                var cust = context.Customers.FirstOrDefault(c => c.id_Customers == customerId);
                if (cust != null)
                {
                    cust.Name = UserName.Text;
                    cust.SurName = SurName.Text;
                    cust.Phone = NumberPhone.Text;
                    cust.Address = Address.Text;
                    cust.id_Payments = Convert.ToInt32(PaymentsComboBox.SelectedValue);

                    context.SaveChanges();
                    MessageBox.Show("Данные были изменены");
                }
                else
                {
                    MessageBox.Show("Пользователь не найден.");
                }
            }
        }

        private async void AddAddress_Click(object sender, RoutedEventArgs e)
        {
            string script = "document.querySelector('#searchbox input').value\r\n";

            if(string.IsNullOrEmpty(script))
            {
                MessageBox.Show("Напиши в строку поиска");
            }
            else
            {
                var result = await webView.CoreWebView2.ExecuteScriptAsync(script);
                Address.Text = result;
            }
        }

        private void UserName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);

        }
        private bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsLetter(c))
                {
                    MessageBox.Show($"Вводить можно только буквы", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }
            return true;
        }

        //Создание отчета по заказам
        private void BtnReportOrder_Click(object sender, RoutedEventArgs e)
        {
            ReportGenerate reportGenerate = new ReportGenerate(loggedInCustomer);
            reportGenerate.GenerateFile(txtNameReport.Text);
            RefreshPage();
        }

        private void BtnOrdersComboBox_Click(object sender, RoutedEventArgs e)
        {
            string selectedReport = (string)ReportOrderComboBox.SelectedItem;

            if (!string.IsNullOrEmpty(selectedReport))
            {
                string filePath = $"\\intStore\\intStore\\bin\\Debug\\Report\\{loggedInCustomer.id_Customers}\\{selectedReport}.docx";

                if (File.Exists(filePath))
                {
                    Process.Start(filePath);
                }
                else
                {
                    MessageBox.Show("Файл не найден.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите отчет.");
            }
        }

        private void RefreshPage()
        {
            string customerDirectory = $"\\intStore\\intStore\\bin\\Debug\\Report\\{loggedInCustomer.id_Customers}";

            ReportOrderComboBox.Items.Clear();

            if (Directory.Exists(customerDirectory))
            {
                var reportFiles = Directory.GetFiles(customerDirectory, "*.docx");
                foreach (var reportFile in reportFiles)
                {
                    ReportOrderComboBox.Items.Add(Path.GetFileNameWithoutExtension(reportFile));
                }
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
