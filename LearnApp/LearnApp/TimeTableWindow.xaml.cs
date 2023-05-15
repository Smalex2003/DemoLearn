
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для TimeTableWindow.xaml
    /// </summary>
    public partial class TimeTableWindow : Window
    {
        public TimeTableWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Метод обновления каждые 30 секунд
        /// </summary>
        public void Refresh30Seconds()
        {
            Thread.Sleep(30000);
            InitializeDG();
            
        }
        /// <summary>
        /// Метод инициализации DataGrid
        /// </summary>
        
        public void InitializeDG()
        {
            ListViewUpcomingEntries.Items.Clear();
            LearnDbEntities db=new LearnDbEntities();
                foreach (ClientService item in db.ClientService)
                {
                    DateTime appointmentDate = item.ServiceTime; // назначенная дата
                    DateTime currentDate = DateTime.Now; // текущая дата и время
                    TimeSpan timeLeft = appointmentDate.Subtract(currentDate);
                    item.timeToStart = timeLeft;
                    if (item.timeToStart <= new TimeSpan(2, 0, 0))
                    {
                        item.Foreground = Brushes.Red;
                    }
                    else
                    {
                        item.Foreground = Brushes.Black;
                        
                    }
                }
                
                int count = db.Service.ToList().Count;
                int countOfListSEcond = db.Client.ToList().Count;
                List<ClientService> list = new List<ClientService>();
                foreach (ClientService item in db.ClientService)
                {
                    if (item.timeToStart <= new TimeSpan(48, 0, 0))
                    {
                        list.Add(item);
                    }
                }
            Dispatcher.Invoke(new Action(() => ListViewUpcomingEntries.ItemsSource = list));
            Thread td = new Thread(new ThreadStart(Refresh30Seconds));
            td.Start();
        }
        /// <summary>
        /// Метод перехода на главное окно
        /// </summary>
        private void ClickBack(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow.mainWindow.Show();
        }
    }
}
