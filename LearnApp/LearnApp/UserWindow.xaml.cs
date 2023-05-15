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
using static LearnApp.MainWindow;

namespace LearnApp
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        List<ServiceToList> listService = new List<ServiceToList>();
        public static List<ServiceToList> sortedList;
        public static MainWindow mainWindow = new MainWindow();
        public UserWindow()
        {
            LearnDbEntities db =new LearnDbEntities();
            InitializeComponent();
            RefreshListViewService();
            sortedList = listService;
            InitializePathsInSortedList();
           




        }
        /// <summary>
        /// Метод инициализации ListView
        /// </summary>
        public void RefreshListViewService()
        {
            listService.Clear();
            LearnDbEntities db = new LearnDbEntities();

            foreach (Service service in db.Service)
            {
                ServiceToList serviceto = new ServiceToList();
                serviceto.Id = service.Id;
                serviceto.Name = service.Name;
                serviceto.Image = service.Image;
                serviceto.Duration = service.Duration;
                serviceto.Cost = service.Cost;
                if (service.Discount > 0) { serviceto.Discount = "Скидка: " + service.Discount.ToString() + "%"; serviceto.ItemBackground = new SolidColorBrush(Colors.LightGreen); }
                else { serviceto.Discount = null; serviceto.ItemBackground = new SolidColorBrush(Colors.White); }
                serviceto.DiscountToSort = service.Discount;
                serviceto.Description = service.Description;
                if (service.Discount > 0) serviceto.OldCost = (service.Cost + (service.Cost * (service.Discount) / 100)).ToString() + " ";
                else serviceto.OldCost = null;
                listService.Add(serviceto);
                ListViewService.ItemsSource = listService;
                TextBlockNumberOfServices.Text = "Кол-во услуг: " + (ListViewService.Items.Count) + "/" + db.Service.ToList().Count;
                
            }
            
        }
        /// <summary>
        /// Метод инициалзиации путей для фотографий
        /// </summary>
        public void InitializePathsInSortedList()
        {
            foreach (var item in sortedList)
            {
                if (!item.Image.Contains("file"))
                {
                    item.Image = @"..\" + item.Image;
                }
            }
        }
        /// <summary>
        /// Метод инициализации ComboBox для фильтра
        /// </summary>
        public void InitializeComboBoxFilter()
        {
            ComboBoxFilter.Items.Add("Все диапазоны");
            ComboBoxFilter.Items.Add("От 0 до 5%");
            ComboBoxFilter.Items.Add("От 5% до 15%");
            ComboBoxFilter.Items.Add("От 15% до 30%");
            ComboBoxFilter.Items.Add("От 30% до 70%");
            ComboBoxFilter.Items.Add("От 70% до 100%");
            ComboBoxFilter.SelectedIndex = 0;
        }
        /// <summary>
        /// Метод добавления всех фильтров
        /// </summary>
        public void AddFilters()
        {
            LearnDbEntities db = new LearnDbEntities();

            sortedList = listService;
            if (ComboBoxSort.SelectedIndex == 2)
            {
                sortedList = sortedList.OrderBy(element => element.Cost).ToList();
            }
            if (ComboBoxSort.SelectedIndex == 1)
            {
                sortedList = sortedList.OrderByDescending(element => element.Cost).ToList();
            }
            if (ComboBoxFilter.SelectedIndex == 1)
            {
                sortedList = sortedList.Where(element => element.DiscountToSort >= 0 && element.DiscountToSort < 5).ToList();
            }
            if (ComboBoxFilter.SelectedIndex == 2)
            {
                sortedList = sortedList.Where(element => element.DiscountToSort >= 5 && element.DiscountToSort < 15).ToList();
            }
            if (ComboBoxFilter.SelectedIndex == 3)
            {
                sortedList = sortedList.Where(element => element.DiscountToSort >= 15 && element.DiscountToSort < 30).ToList();
            }
            if (ComboBoxFilter.SelectedIndex == 4)
            {
                sortedList = sortedList.Where(element => element.DiscountToSort >= 30 && element.DiscountToSort < 70).ToList();
            }
            if (ComboBoxFilter.SelectedIndex == 5)
            {
                sortedList = sortedList.Where(element => element.DiscountToSort >= 70 && element.DiscountToSort < 100).ToList();
            }
            if (TextBoxFind.Text != "")
            {
                sortedList = sortedList.Where(element => element.Name.Contains(TextBoxFind.Text)).ToList();
            }
            RefreshListViewService();
            ListViewService.ItemsSource = sortedList;
            TextBlockNumberOfServices.Text = "Кол-во услуг: " + (ListViewService.Items.Count) + "/" + db.Service.ToList().Count;
            sortedList = listService;

        }
        /// <summary>
        /// Обработка события изменения SelectedItem для применения фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            AddFilters();
        }
      
        /// <summary>
        /// Метод возврата на предыдущее окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickLeave(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        /// <summary>
        /// Метод перехода на окно администратора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChangeGetAdmin(object sender, TextChangedEventArgs e)
        {
            if (TextBoxAdminCode.Text == "0000")
            {
                LearnDbEntities db = new LearnDbEntities();
                MessageBox.Show("Вы перешли в режим администратора");
                TextBoxAdminCode.Text = "";
                this.Hide();
                mainWindow.RefreshListViewService();
                mainWindow.Show();
            }
        }
        /// <summary>
        /// Метод для изменения текса в строке поиска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxFind_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddFilters();
        }
    }
}
