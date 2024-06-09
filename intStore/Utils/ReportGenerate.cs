using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Image = Xceed.Document.NET.Image;

namespace intStore.Utils
{
    public class ReportGenerate
    {
        private Customers loggedInCustomer;
        public ReportGenerate(Customers customer)
        {
            loggedInCustomer = customer;
        }
        public void GenerateFile(string txtNameReport)
        {
            string customerDirectory = $"\\intStore\\intStore\\bin\\Debug\\Report\\{loggedInCustomer.id_Customers}";
            if (!Directory.Exists(customerDirectory))
            {
                Directory.CreateDirectory(customerDirectory);
            }

            if (string.IsNullOrWhiteSpace(txtNameReport))
            {
                MessageBox.Show("Данное поле не может быть пустым, дайте наименование файлу!", "Название файла...", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                return;
            }

            string txtName = txtNameReport;
            string filePath = $"{customerDirectory}\\{txtName}.docx";

            if (File.Exists(filePath))
            {
                MessageBox.Show("Такое название уже есть, придумайте новое");
                return;
            }

            txtNameReport = "";

            using (var doc = DocX.Create(filePath))
            {
                doc.InsertParagraph("Интернет-Магазин").Color(Color.FromArgb(255, 51, 187, 94)).FontSize(36d).Bold().Alignment = Alignment.center;

                doc.InsertParagraph("");
                doc.InsertParagraph("");

                doc.InsertParagraph($"Имя клиента: {loggedInCustomer.Name}");
                doc.InsertParagraph($"Выбор оплаты: {loggedInCustomer.Payments.MethodName}");
                doc.InsertParagraph($"Дата отчета: {DateTime.Now:yyyy.MM.dd}");


                using (var context = new InternetStoreEntities1())
                {
                    var cars = context.Cart.Where(c => c.id_Customer == loggedInCustomer.id_Customers && c.id_Status == 2).Take(60).ToList();

                    var orderCount = context.Cart.Count(o => o.id_Customer == loggedInCustomer.id_Customers);
                    var totalAmount = context.Cart
                        .Where(o => o.id_Customer == loggedInCustomer.id_Customers)
                        .Sum(o => o.Orders.TotalAmount);

                    doc.InsertParagraph($"Количество заказов: {orderCount}").FontSize(10d).Bold().Alignment = Alignment.left;
                    doc.InsertParagraph($"Общая стоимость: {totalAmount:F2} руб.").FontSize(10d).Bold().Alignment = Alignment.left;

                    if (cars.Any())
                    {
                        var table = doc.AddTable(cars.Count + 1, 6);

                        Color headerColor = ColorTranslator.FromHtml("#33bb5e");

                        table.Rows[0].Cells[0].Paragraphs.First().Append("Номер заказа").Color(Color.White).Bold();
                        table.Rows[0].Cells[1].Paragraphs.First().Append("Дата").Color(Color.White).Bold();
                        table.Rows[0].Cells[2].Paragraphs.First().Append("Стоимость").Color(Color.White).Bold();
                        table.Rows[0].Cells[3].Paragraphs.First().Append("Количество").Color(Color.White).Bold();
                        table.Rows[0].Cells[4].Paragraphs.First().Append("Название продукта").Color(Color.White).Bold();
                        table.Rows[0].Cells[5].Paragraphs.First().Append("Категория").Color(Color.White).Bold();

                        foreach (var cell in table.Rows[0].Cells)
                        {
                            cell.FillColor = headerColor;
                        }

                        int rowIndex = 1;

                        foreach (var car in cars)
                        {
                            if (car.Orders?.id_Order != null)
                            {
                                table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(car.Orders.id_Order.ToString()).Alignment = Alignment.center;
                                table.Rows[rowIndex].Cells[1].Paragraphs.First().Append($"{car.Orders.OrderDate:dd.MM.yyyy}").Alignment = Alignment.center;
                                table.Rows[rowIndex].Cells[2].Paragraphs.First().Append($"{car.Orders.TotalAmount:F2} руб.").Alignment = Alignment.center;
                                table.Rows[rowIndex].Cells[3].Paragraphs.First().Append($"{car.Quantity} шт.").Alignment = Alignment.center;
                                table.Rows[rowIndex].Cells[4].Paragraphs.First().Append(car.OrdersWithCart.Goods.NameProduct.ToString()).Alignment = Alignment.center;
                                table.Rows[rowIndex].Cells[5].Paragraphs.First().Append(car.OrdersWithCart.Goods.Categories.NameCategories.ToString()).Alignment = Alignment.center;

                                rowIndex++;
                            }
                        }


                        doc.InsertTable(table).Alignment = Alignment.center;

                        doc.InsertParagraph("");


                        doc.InsertParagraph("Информация о всех заказах, которые были оформлены и выполнены в нашем интернет-магазине.").FontSize(18d).Alignment = Alignment.center;


                        doc.InsertParagraph("");
                        doc.InsertParagraph("");

                        string signaturePath = @"C:\Users\ultra\source\repos\intStore\intStore\Images\Obrazec.png";
                        Image signatureImage = doc.AddImage(signaturePath);
                        Picture signaturePicture = signatureImage.CreatePicture(150, 150);

                        string signaturePath1 = @"C:\Users\ultra\source\repos\intStore\intStore\Images\Podpic.png";
                        Image signatureImage1 = doc.AddImage(signaturePath1);
                        Picture signaturePicture1 = signatureImage1.CreatePicture(60, 60);

                        var directorParagraph = doc.InsertParagraph();
                        directorParagraph.Append("Генеральный директор: ").Bold().FontSize(12);
                        directorParagraph.Append("Макаров М.Ю.").FontSize(12).AppendPicture(signaturePicture1);
                        directorParagraph.SpacingAfter(20);

                        doc.InsertParagraph("");
                        doc.InsertParagraph("");

                        var accountantParagraph = doc.InsertParagraph();
                        accountantParagraph.Append("Генеральный бухгалтер: ").Bold().FontSize(12);
                        accountantParagraph.Append("Макарова А.Ю.").FontSize(12).AppendPicture(signaturePicture1);

                        var paragraph1 = doc.InsertParagraph();
                        paragraph1.AppendPicture(signaturePicture).Alignment = Alignment.right;

                        var paragraph2 = doc.InsertParagraph();





                    }
                    else
                    {
                        doc.InsertParagraph("Нет данных об заказах.");
                    }
                }

                doc.Save();
                Process.Start(filePath);
            }
        }
    }
}
