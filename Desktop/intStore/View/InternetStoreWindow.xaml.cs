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
using System.Collections.ObjectModel;
using intStore.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
        public ObservableCollection<Product> cartItemsWithImages = new ObservableCollection<Product>();
        private ObservableCollection<Product> orderItemsWithImages = new ObservableCollection<Product>();
        public InternetStoreWindow(Customers customers)
        {
            InitializeComponent();
            LoadProductList();
            LoadCategoriesList();
            loggedInCustomer = customers;
            LoadCartItemsForUser(loggedInCustomer.id_Customers);
            LoadOrderItemsForUser(loggedInCustomer.id_Customers);
            UpdateCartButton();
            LoadedText(customers);
            LoadFilterPrice();
        }

        // Общая стоимость товара в корзине
        private void RecalculateTotalPrice()
        {

            decimal totalPrice = Convert.ToDecimal(cartItemsWithImages.Sum(item => item.Price * item.Quantity));
            Cost.Text = $"₽{totalPrice:F2}";


            if (totalPrice > 1000)
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

        // Привязка данных к CollectionViewSource и подключение фильтрации
        private void LoadFilterPrice()
        {
            var viewSource = (CollectionViewSource)this.Resources["ProductCollection"];
            viewSource.Source = productList;

            var collectionView = CollectionViewSource.GetDefaultView(productList);
            collectionView.Filter = ProductCollection_Filter;
        }

        // Данные для личного кабинета
        private void LoadedText(Customers customers)
        {
            DateReg.Text = $"{customers.RegisterDate:yyyy.MM.dd}";
            NumberPhone.Text = $"{customers.Phone}";
            Address.Text = $"{customers.Address}";
            UserName.Text = $"{customers.Name}";
            SurName.Text = $"{customers.SurName}";
        }

        private bool IsValidNumberPhone(string phone)
        {
            string pattern = @"^(\+7|8)?[\s-]?\(?\d{3}\)?[\s-]?\d{3}[\s-]?\d{2}[\s-]?\d{2}$";
            return Regex.IsMatch(phone, pattern);
        }

        // Обновление счётчика корзины...
        private void UpdateCartButton()
        {
            int itemsInCartCount;
            using (var context = new InternetStoreEntities1())
            {
                itemsInCartCount = context.Cart.Count(item => item.id_Customer == loggedInCustomer.id_Customers && item.Status.NameStatus == "Не оформлен");
            }
            btnCart.Text = itemsInCartCount.ToString();
        }

        // Отображение товаров
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

        // Отображение данных в корзине... 
        private void LoadCartItemsForUser(int customerId)
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            using (var context = new InternetStoreEntities1())
            {
                var cartItemsForUser = context.Cart
                    .Where(item => item.id_Customer == customerId && item.Status.NameStatus == "Не оформлен")
                    .Select(item => new
                    {
                        IdProduct = item.OrdersWithCart.id_OrderWithCart,
                        ImageProductData = item.OrdersWithCart.Goods.ImageProduct,
                        NameProduct = item.OrdersWithCart.Goods.NameProduct,
                        Quantity = item.Quantity,
                        Price = item.OrdersWithCart.Goods.Price,
                        Status = item.Status.NameStatus,

                    }).ToList();

                cartItemsWithImages.Clear();

                foreach (var item in cartItemsForUser)
                {
                    Product product = new Product
                    {
                        id_Product = item.IdProduct,
                        ImageProduct = imageManipulation.GetPhotoFromDataBase(item.ImageProductData),
                        NameProduct = item.NameProduct,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Status = item.Status,

                    };
                    cartItemsWithImages.Add(product);
                }

                CartList.ItemsSource = cartItemsWithImages;

                RecalculateTotalPrice();
            }
        }

        // Отображение данных в заказах
        private void LoadOrderItemsForUser(int customerId)
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            using (var context = new InternetStoreEntities1())
            {
                var cartItemsForUser = context.Cart
                    .Where(item => item.id_Customer == customerId && item.Status.NameStatus == "Оформлен")
                    .Select(item => new
                    {
                        IdProduct = item.OrdersWithCart.Goods.id_Product,
                        ImageProductData = item.OrdersWithCart.Goods.ImageProduct,
                        NameProduct = item.OrdersWithCart.Goods.NameProduct,
                        Quantity = item.Quantity,
                        Price = item.OrdersWithCart.Goods.Price,
                        Status = item.Status.NameStatus,
                        DateOrder = item.Orders.OrderDate,

                    }).ToList();

                orderItemsWithImages.Clear();

                foreach (var item in cartItemsForUser)
                {
                    Product product = new Product
                    {
                        id_Product = item.IdProduct,
                        ImageProduct = imageManipulation.GetPhotoFromDataBase(item.ImageProductData),
                        NameProduct = item.NameProduct,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Status = item.Status,
                        orderDate = item.DateOrder
                    };
                    orderItemsWithImages.Add(product);
                }

                OrdersList.ItemsSource = orderItemsWithImages;

                RecalculateTotalPrice();
            }
        }

        // Отображение категории
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


        // Методы для кнопок с хранимыми процедурами внутри
        private void AddProductToCartHandler(object sender, ProductEventArgs e)
        {
            using (var context = new InternetStoreEntities1())
            {
                // Проверяем, есть ли уже товар с таким идентификатором в корзине
                var existingCartItem = context.Cart.FirstOrDefault(item => item.OrdersWithCart.id_Product == e.Product.id_Product && item.Status.NameStatus == "Не оформлен");
                if (existingCartItem != null)
                {
                    // Если товар уже есть в корзине, увеличиваем количество
                    existingCartItem.Quantity += e.Quantity;
                    context.SaveChanges();
                    MessageBox.Show("Количество товара в корзине обновлено!");
                }
                else
                {
                    // Если товара нет в корзине, добавляем новый элемент
                    AddProductToCart(loggedInCustomer.id_Customers, e.Product.id_Product, e.Quantity);
                    MessageBox.Show("Товар был добавлен в корзину!");
                }
            }

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

        private void UpdateProductQuantity(int productId, int quantity)
        {
            using (var context = new InternetStoreEntities1())
            {
                var product = context.Goods.FirstOrDefault(p => p.id_Product == productId);
                if (product != null)
                {
                    product.Quantity -= quantity;
                    context.SaveChanges();
                }
            }
        }

        private void BuyProduct(int customerId, int cartProductId)
        {
            using (var context = new InternetStoreEntities1())
            {
                context.Database.ExecuteSqlCommand("EXEC MoveCartItemToOrder @CustomerId, @OrderWithCatItemId",
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@OrderWithCatItemId", cartProductId));
            }
        }

        // Добавление товара в корзину, удаление
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

        // Покупка товара, данные отправляется в заказы
        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(NumberPhone.Text))
            {
                MessageBox.Show($"Заказ нельзя оформить так, как не был указан номер телефона в личном кабинете!", "Данные пусты", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Вы точно хотите приобрести все товары в корзине?", "Покупка товаров", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
        
                foreach (var product in cartItemsWithImages)
                {
                    BuyProduct(loggedInCustomer.id_Customers, product.id_Product);
                }

                InformationPageOrders informationPageOrders = new InformationPageOrders(loggedInCustomer, this);
                informationPageOrders.ShowDialog();

                cartItemsWithImages.Clear();
                CartList.ItemsSource = cartItemsWithImages;

                RecalculateTotalPrice();
                LoadOrderItemsForUser(loggedInCustomer.id_Customers);
                UpdateCartButton();
            }
        }

        // Выбор определенной категории с товарами 
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


        // Кнопки для перехода между вкладками TabControl...
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
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidNumberPhone(NumberPhone.Text))
            {
                MessageBox.Show("Некорректно введен номер телефона. Пример: 89085912345 ");
                return;
            }

            int customerId = loggedInCustomer.id_Customers;
            string newPhone = NumberPhone.Text;

            using (var context = new InternetStoreEntities1())
            {
                // Проверка, существует ли уже такой номер телефона у другого пользователя
                var existingCustomer = context.Customers
                    .FirstOrDefault(c => c.Phone == newPhone && c.id_Customers != customerId);

                if (existingCustomer != null)
                {
                    MessageBox.Show("Этот номер телефона уже используется другим пользователем.");
                    return;
                }

                // Получаем текущего пользователя
                var cust = context.Customers.FirstOrDefault(c => c.id_Customers == customerId);
                if (cust != null)
                {
                    cust.Name = UserName.Text;
                    cust.SurName = SurName.Text;
                    cust.Phone = newPhone;
                    cust.Address = Address.Text;

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
            reportGenerate.GenerateFile();
        }

        //Кнопки уменьшения и увеличения товара в корзине
        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var textBox = button.Tag as TextBox;
                if (textBox != null)
                {
                    if (int.TryParse(textBox.Text, out int quantity))
                    {
                        if (quantity > 1)
                        {
                            quantity--;
                            textBox.Text = quantity.ToString();

                            var product = button.DataContext as Product;
                            if (product != null)
                            {
                                product.Quantity = quantity;
                                SaveChangesToDatabase(product);
                                LoadCartItemsForUser(loggedInCustomer.id_Customers);
                                RecalculateTotalPrice();
                            }
                        }
                    }
                }
            }
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var textBox = button.Tag as TextBox;
                if (textBox != null)
                {
                    if (int.TryParse(textBox.Text, out int quantity))
                    {
                        quantity++;
                        textBox.Text = quantity.ToString();

                        var product = button.DataContext as Product;
                        if (product != null)
                        {
                            product.Quantity = quantity;
                            SaveChangesToDatabase(product);
                            LoadCartItemsForUser(loggedInCustomer.id_Customers);
                            RecalculateTotalPrice();
                        }
                    }
                }
            }
            
        }

        private void SaveChangesToDatabase(Product product)
        {
            using (var context = new InternetStoreEntities1())
            {
                var cartItem = context.Cart.FirstOrDefault(c => c.id_OrdersWithCart == product.id_Product);
                if (cartItem != null)
                {
                    cartItem.Quantity = product.Quantity;

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении изменений в базе данных: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Товар не найден в корзине.");
                }
            }
        }

        // Обработчик события для изменения нижнего значения ползунка цены
        private void PriceSlider_LowerValueChanged(object sender, RoutedEventArgs e)
        {
            MinPriceTextBox.Text = PriceSlider.LowerValue.ToString();
            ApplyFilters();
        }

        // Обработчик события для изменения верхнего значения ползунка цены
        private void PriceSlider_HigherValueChanged(object sender, RoutedEventArgs e)
        {
            MaxPriceTextBox.Text = PriceSlider.HigherValue.ToString();
            ApplyFilters();
        }

        // Применение фильтра и обновление товаров 
        private void ApplyFilters()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsList.ItemsSource);
            if (view != null)
            {
                view.Filter = ProductCollection_Filter;
                view.Refresh();
            }
        }

        // Сброс фильтра цены
        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            MinPriceTextBox.Text = "0";
            MaxPriceTextBox.Text = "10000";
            PriceSlider.LowerValue = 0;
            PriceSlider.HigherValue = 10000;
            ApplyFilters();
        }

        // Логика фильтрации для имени и цены
        private bool ProductCollection_Filter(object item)
        {
            var product = item as Product;
            if (product != null)
            {
                string filterText = txtSearchCart.Text.ToLower();
                decimal minPrice = 0;
                decimal maxPrice = decimal.MaxValue;

                if (!string.IsNullOrEmpty(MinPriceTextBox.Text))
                    decimal.TryParse(MinPriceTextBox.Text, out minPrice);

                if (!string.IsNullOrEmpty(MaxPriceTextBox.Text))
                    decimal.TryParse(MaxPriceTextBox.Text, out maxPrice);

                bool matchesName = string.IsNullOrEmpty(filterText) || product.NameProduct.ToString().ToLower().Contains(filterText);
                bool matchesPrice = product.Price >= minPrice && product.Price <= maxPrice;

                return matchesName && matchesPrice;
            }
            return false;
        }
    }
}

