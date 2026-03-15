using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NikolaevaLib;

namespace NikolaevaWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApplicationContext _databaseContext; // Контекст базы данных
        private Service _partnerManager; // Сервис для управления партнёрами
        public MainWindow()
        {
            InitializeComponent();
            _databaseContext = new ApplicationContext(); // Инициализация контекста базы данных
            _partnerManager = new Service(); // Инициализация сервиса для работы с партнёрами
            PartnersList.SelectionChanged += (s, e) => RefreshSalesView(); // Обработчик изменения выбора партнёра
            RefreshPartnersView(); // Обновление списка партнёров
            RefreshSalesView(); // Обновление списка продаж
        }

        // Метод для обновления отображения партнёров
        private void RefreshPartnersView()
        {
            var currentSelectedPartner = PartnersList.SelectedItem as PartnerView; // Получаем текущего выбранного партнёра
            _partnerManager.UpdateDiscounts(_databaseContext); // Обновляем скидки
            var partnerCollection = _partnerManager.LoadPartners(_databaseContext); // Загружаем список партнёров
            PartnersList.ItemsSource = partnerCollection; // Обновляем источник данных для списка партнёров
            if (currentSelectedPartner != null)
            {
                PartnersList.SelectedItem = partnerCollection.FirstOrDefault(p => p.Name == currentSelectedPartner.Name); // Восстанавливаем выбор партнёра, если он был
            }
        }

        // Метод для выхода из приложения
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Закрываем окно
        }

        // Метод для обновления отображения продаж
        private void RefreshSalesView()
        {
            var currentSelectedPartner = PartnersList.SelectedItem as PartnerView; // Получаем текущего выбранного партнёра
            if (currentSelectedPartner != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name); // Загружаем данные партнёра
                if (partnerData != null)
                {
                    SalesDataGrid.ItemsSource = _partnerManager.GetSales(_databaseContext, partnerData); // Загружаем и отображаем продажи
                }
            }
            else
            {
                SalesDataGrid.ItemsSource = null; // Если партнёр не выбран, очищаем список продаж
            }
        }

        // Метод для добавления нового партнёра
        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            PartnerWindow partnerDialog = new PartnerWindow(_databaseContext); // Создаём диалог для добавления партнёра
            partnerDialog.Owner = this; // Устанавливаем родительское окно
            if (partnerDialog.ShowDialog() == true) // Если пользователь добавил партнёра
            {
                RefreshPartnersView(); // Обновляем список партнёров
            }
        }

        // Метод для редактирования существующего партнёра
        private void EditPartner_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPartner = PartnersList.SelectedItem as PartnerView; // Получаем текущего выбранного партнёра
            if (currentSelectedPartner != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name); // Загружаем данные партнёра
                PartnerWindow partnerDialog = new PartnerWindow(_databaseContext, partnerData); // Создаём диалог для редактирования партнёра
                partnerDialog.Owner = this;
                if (partnerDialog.ShowDialog() == true) // Если пользователь сохранил изменения
                {
                    RefreshPartnersView(); // Обновляем список партнёров
                }
            }
        }

        // Метод для удаления партнёра
        private void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPartner = PartnersList.SelectedItem as PartnerView; // Получаем текущего выбранного партнёра
            Window parentWindow = Window.GetWindow(this); // Получаем родительское окно
            if (currentSelectedPartner != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name); // Загружаем данные партнёра
                var confirmationResult = MessageBox.Show(parentWindow, "Вы действительно хотите удалить выбранного партнера?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question); // Подтверждение удаления
                if (confirmationResult == MessageBoxResult.Yes)
                {
                    _partnerManager.DeletePartner(_databaseContext, partnerData); // Удаляем партнёра
                }
            }
            RefreshPartnersView(); // Обновляем список партнёров
        }

        // Метод для добавления новой продажи
        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPartner = PartnersList.SelectedItem as PartnerView; // Получаем текущего выбранного партнёра
            if (currentSelectedPartner != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name); // Загружаем данные партнёра
                if (partnerData != null)
                {
                    SaleWindow saleDialog = new SaleWindow(_databaseContext, null, partnerData); // Создаём диалог для добавления продажи
                    saleDialog.Owner = this;
                    if (saleDialog.ShowDialog() == true) // Если продажа добавлена
                    {
                        RefreshSalesView(); // Обновляем список продаж
                        RefreshPartnersView(); // Обновляем список партнёров
                    }
                }
            }
        }

        // Метод для редактирования существующей продажи
        private void EditSale_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPartner = PartnersList.SelectedItem as PartnerView; // Получаем текущего выбранного партнёра
            var currentSelectedSale = SalesDataGrid.SelectedItem as Sale; // Получаем текущую выбранную продажу
            if (currentSelectedPartner != null && currentSelectedSale != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name); // Загружаем данные партнёра
                if (partnerData != null)
                {
                    SaleWindow saleDialog = new SaleWindow(_databaseContext, currentSelectedSale, partnerData); // Создаём диалог для редактирования продажи
                    saleDialog.Owner = this;
                    if (saleDialog.ShowDialog() == true) // Если продажа отредактирована
                    {
                        RefreshSalesView(); // Обновляем список продаж
                        RefreshPartnersView(); // Обновляем список партнёров
                    }
                }
            }
        }

        // Метод для удаления продажи
        private void DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedSale = SalesDataGrid.SelectedItem as Sale; // Получаем текущую выбранную продажу
            if (currentSelectedSale != null)
            {
                var confirmationResult = MessageBox.Show("Вы действительно хотите удалить выбранную продажу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question); // Подтверждение удаления
                if (confirmationResult == MessageBoxResult.Yes)
                {
                    _partnerManager.DeleteSale(_databaseContext, currentSelectedSale); // Удаляем продажу
                    RefreshSalesView(); // Обновляем список продаж
                    RefreshPartnersView(); // Обновляем список партнёров
                }
            }
        }
    }
}
