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


namespace intStore.View
{
    /// <summary>
    /// Interaction logic for InternetStoreWindow.xaml
    /// </summary>
    public partial class InternetStoreWindow : Window
    {
        private List<testProduct> productsList;
        public InternetStoreWindow()
        {
            InitializeComponent();
            LoadProductList();
            productsList = new List<testProduct>();
            var Current = InternetStoreEntities1.GetContext().Goods.ToList();

            var imageProducts = InternetStoreEntities1.GetContext().Goods.Select(p => p.ImageProduct).FirstOrDefault();

            if (!string.IsNullOrEmpty(imageProducts))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageProducts, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                testImage.Source = bitmap;
            }


            
        }
        private void LoadProductList()
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            var productsList = new List<testProduct>(); // Инициализируем список
            var Current = InternetStoreEntities1.GetContext().Goods.ToList();

            if (Current == null || !Current.Any())
            {
                // Логика обработки случая, когда Current пустой
                return;
            }

            foreach (var v in Current)
            {
                if (v == null) continue; // Пропускаем null-объекты в коллекции

                testProduct product = new testProduct()
                {
                    id_Product = v.id_Product,
                    NameProduct = v.NameProduct,
                    Description = v.Description,
                    Weight = v.Weight,
                    Price = v.Price,
                    Quantity = v.Quantity,
                    DateProduct = v.DateProduct,
                    ImageProduct = imageManipulation.GetPhotoFromDataBase(v.ImageProduct)
                };

                productsList.Add(product);
            }

            ItemsList.ItemsSource = productsList;
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
    }
}
