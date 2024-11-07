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

namespace lab3
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
        
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if(SelectedService != null)
                this._currentService = SelectedService;

            DataContext = _currentService;

            var _currentClient = Lapitskaya_autoserviceEntities.GetContext().Client.ToList();
            ComboClient.ItemsSource = _currentClient;
        }

        private ClientService _currentClientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            string startTimeInput = TBStart.Text;
            DateTime startTime;

            if (!DateTime.TryParse(startTimeInput, out startTime))
            {
                errors.AppendLine("Некорректное время");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);
            

            if (_currentClientService.ID == 0)
                Lapitskaya_autoserviceEntities.GetContext().ClientService.Add(_currentClientService);

            try
            {
                Lapitskaya_autoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length < 5 || !s.Contains(':'))
                TBEnd.Text = "";
            else
            {

                string startTimeInput = TBStart.Text;

                DateTime startTime;


                if (DateTime.TryParse(startTimeInput, out startTime))
                {
                    string[] start = s.Split(new char[] { ':' });
                    int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                    int startMin = Convert.ToInt32(start[1].ToString());

                    int sum = startHour + startMin + _currentService.Duration;

                    int endHour = (sum / 60) % 24;
                    int endMin = sum % 60;
                    s = endHour.ToString() + ":" + endMin.ToString();
                    TBEnd.Text = s;

                }
                else
                {
                    MessageBox.Show("Некорректное время");
                }
            }
        }
    }
}
