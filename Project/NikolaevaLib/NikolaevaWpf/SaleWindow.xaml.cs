using NikolaevaLib;
using System;
using System.Collections.Generic;
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

namespace NikolaevaWpf
{
    /// <summary>
    /// Логика взаимодействия для SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        private readonly ApplicationContext _databaseContext; // Контекст базы данных
        private Sale _currentSale; // Текущая продажа (для редактирования или добавления)
        private bool _isEditing; // Флаг, указывающий, редактируется ли продажа
        private readonly Service _partnerManager; // Сервис для работы с данными
        private Partner _currentPartner; // Партнёр, с которым связана продажа

        // Конструктор, который принимает контекст базы данных, продажу и партнёра
        public SaleWindow(ApplicationContext context, Sale sale = null, Partner partner = null)
        {
            InitializeComponent();
            _databaseContext = context;
            _partnerManager = new Service();
            _currentPartner = partner ?? throw new ArgumentNullException(nameof(partner), "Партнер не выбран.");

            // Если продажа передана, это режим редактирования
            if (sale != null)
            {
                _isEditing = true;
                _currentSale = sale;
                Title = "Редактирование продукта"; // Заголовок окна для редактирования
                LoadSaleDetails(); // Загружаем данные для редактирования
            }
            else
            {
                _isEditing = false;
                Title = "Добавление продукта"; // Заголовок окна для добавления новой продажи
                _currentSale = new Sale(); // Создаём новый объект продажи
            }
        }

        // Метод для загрузки данных продажи в соответствующие поля формы
        private void LoadSaleDetails()
        {
            ProductNameTextBox.Text = _currentSale.ProductName;
            QuantityTextBox.Text = _currentSale.Quantity.ToString();
            SaleDatePicker.SelectedDate = _currentSale.Date;
        }

        // Метод для обработки кнопки сохранения данных продажи
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка, что наименование продукта не пустое
                if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                {
                    MessageBox.Show("Введите корректное наименование продукта.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка, что количество положительное число
                if (!int.TryParse(QuantityTextBox.Text, out int saleQuantity) || saleQuantity <= 0)
                {
                    MessageBox.Show("Количество должно быть положительным числом.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка, что дата продажи выбрана и не в будущем
                if (SaleDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату продажи.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (SaleDatePicker.SelectedDate > DateTime.Today)
                {
                    MessageBox.Show("Дата продажи не может быть в будущем.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохранение данных в объект продажи
                _currentSale.ProductName = ProductNameTextBox.Text;
                _currentSale.Quantity = saleQuantity;
                _currentSale.Date = SaleDatePicker.SelectedDate.Value;
                _currentSale.Partnerid = _currentPartner.Id;

                // Если редактируем существующую продажу
                if (_isEditing)
                {
                    _partnerManager.UpdateSale(_databaseContext, _currentSale); // Обновляем продажу в базе
                }
                else
                {
                    _partnerManager.AddSale(_databaseContext, _currentSale); // Добавляем новую продажу
                }

                _partnerManager.SaveChanges(_databaseContext); // Сохраняем изменения в базе данных
                DialogResult = true; // Закрытие окна с результатом успешного сохранения
                Close();
            }
            catch (Exception error)
            {
                MessageBox.Show($"Произошла ошибка: {error.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); // Обработка ошибок
            }
        }

        // Метод для обработки кнопки отмены
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрытие окна с результатом отмены
            Close();
        }
    }
}

