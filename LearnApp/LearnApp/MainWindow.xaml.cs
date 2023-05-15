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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LearnApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ServiceToList> listService = new List<ServiceToList>();
        public static List<ServiceToList> sortedList;
        public static MainWindow mainWindow;
        public static AddServiceWindow windowAddService = new AddServiceWindow();
        public static ChangeServiceWindow windowChangeService = new ChangeServiceWindow();
        public static AddServiceClientWindow windowAddClientToService = new AddServiceClientWindow();
        public static ServiceToList currentChoosedService;
        public static Service editservice;
        public static TimeTableWindow windowUpcomingEntries = new TimeTableWindow();
        public MainWindow()
        {
            LearnDbEntities db=new LearnDbEntities();
            InitializeComponent();
            RefreshListViewService();
            


            mainWindow = this;
            
        }
        public class ServiceToList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
            public int Duration { get; set; }
            public int Cost { get; set; }
            public string Discount { get; set; }
            public string Description { get; set; }
            public string OldCost { get; set; }
            public int DiscountToSort { get; set; }
            public SolidColorBrush ItemBackground { get; set; }
        }
        /// <summary>
        /// Метод обновления ListView
        /// </summary>
        public void RefreshListViewService()
        {
            listService.Clear();
            LearnDbEntities db = new LearnDbEntities();
            
            foreach(Service service in db.Service)
            {
                ServiceToList serviceto = new ServiceToList();
                serviceto.Id= service.Id;
                serviceto.Name= service.Name;
                serviceto.Image= service.Image;
                serviceto.Duration= service.Duration;
                serviceto.Cost = service.Cost;
                if (service.Discount > 0) { serviceto.Discount = "Скидка: " + service.Discount.ToString() + "%"; serviceto.ItemBackground = new SolidColorBrush(Colors.LightGreen); }
                else { serviceto.Discount = null; serviceto.ItemBackground = new SolidColorBrush(Colors.White); }
                    serviceto.DiscountToSort = service.Discount;
                serviceto.Description= service.Description;
                if (service.Discount > 0) serviceto.OldCost = (service.Cost + (service.Cost * (service.Discount) / 100)).ToString()+" ";
                else serviceto.OldCost = null;
                listService.Add(serviceto);
                ListViewService.ItemsSource = listService;
                TextBlockNumberOfServices.Text = "Кол-во услуг: " + (ListViewService.Items.Count) + "/" + db.Service.ToList().Count;
               
            }
            
        }
        /// <summary>
        /// Метод инициализации ComboBoxFilter
        /// </summary>
        public void InitializeFilterCb()
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
                    sortedList = sortedList.Where(element =>element.Name.Contains(TextBoxFind.Text)).ToList();
                }
                RefreshListViewService();
                ListViewService.ItemsSource = sortedList;
                TextBlockNumberOfServices.Text = "Кол-во услуг: " + (ListViewService.Items.Count) + "/" + db.Service.ToList().Count;
                
            
        }

        /// <summary>
        /// Метод для кнопки редактирования услуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            LearnDbEntities db = new LearnDbEntities();
            ServiceToList choosedService = (sender as Button).DataContext as ServiceToList;
            Service currentService;
            
                currentService = db.Service.Where(element => element.Id == choosedService.Id).FirstOrDefault();
                windowChangeService.Show();
                windowChangeService.TextBoxCost.Text = currentService.Cost.ToString();
                windowChangeService.TextBoxDiscount.Text = currentService.Discount.ToString();
                windowChangeService.TextBoxDuration.Text = currentService.Duration.ToString();
                windowChangeService.TextBoxName.Text = currentService.Name;
                windowChangeService.ListViewPhotosCurrent.Items.Clear();
                windowChangeService.TextBoxId.Text = currentService.Id.ToString();
                editservice = currentService;
                if (currentService.OptionalImage.Count > 0)
                {
                    MessageBox.Show("Additional imgaes count: " + currentService.OptionalImage.Count);
                    foreach (var image in currentService.OptionalImage)
                    {
                        Service service = new Service();
                        service.Image = image.Image;
                        windowChangeService.ListViewPhotosCurrent.Items.Add(service);
                    }
                }
                if (currentService.Image.Contains("file"))
                {
                    windowChangeService.ImageChoosedPhoto.Source = new BitmapImage(new Uri(currentService.Image));
                }
                else
                {
                    windowChangeService.ImageChoosedPhoto.Source = new BitmapImage(new Uri(System.IO.Path.Combine("C:\\Users\\Azarch\\source\\repos\\SchoolService\\SchoolService\\", currentService.Image)));
                }
            
        }
        /// <summary>
        /// Метод для кнопки удаления услуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            LearnDbEntities db = new LearnDbEntities();
            ServiceToList currentService = (sender as Button).DataContext as ServiceToList;
            Service element = db.Service.Where(serviceFind => serviceFind.Id == currentService.Id).FirstOrDefault();
            if (element.ClientService.Count > 0)
            {
                MessageBox.Show("Вы не можете удалить эту услугу так как на неё есть запись!");
                return;
            }
            db.Service.Remove(element);
            db.SaveChanges();
            AddFilters();
            MessageBox.Show("Услуга была успешно удалена!");
        }
        /// <summary>
        /// Метод для кнопки записи на услугу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZapisBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            LearnDbEntities db = new LearnDbEntities();
            windowAddClientToService.Show();
            currentChoosedService = (sender as Button).DataContext as ServiceToList;
            windowAddClientToService.TextBlockDuration.Text = "Длительность услуги: " + currentChoosedService.Duration.ToString();
            windowAddClientToService.TextBlockName.Text = "Наименование услуги: " + currentChoosedService.Name.ToString();
            windowAddClientToService.ComboBoxChooseClient.ItemsSource = db.Client.ToList();

            
        }
        /// <summary>
        /// Метод для кнопки перехода на окно добавления услуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
           windowAddService.ClearTb();
            windowAddService.Show();
        }
        /// <summary>
        /// Метод для кнопки перехода на окно записей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoonZapisiBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
           windowUpcomingEntries.Show();
            windowUpcomingEntries.InitializeDG();
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

        /// <summary>
        /// Метод для смены значения в combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            AddFilters();
        }


    }

}
