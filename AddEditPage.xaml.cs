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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        
        
        
        private Service _currentService = new Service();
        public AddEditPage(Service SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
                _currentService = SelectedService;

            DataContext = _currentService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Title))
                errors.AppendLine("Укажите название услуги");

            if (_currentService.Cost <= 0)
                errors.AppendLine("Укажите стоимость услуги");

            if (_currentService.DiscountInt < 0)
                errors.AppendLine("Скидка не может быть меньше 0");

            if (_currentService.DiscountInt > 100)
                errors.AppendLine("Скидка не может быть больше 100");

            /*if (_currentService.DiscountInt == null)
                errors.AppendLine("Укажите скидку");*/

            if (_currentService.Duration <= 0)
                errors.AppendLine("Укажите длительность услуги");

            if (_currentService.Duration > 240)
                errors.AppendLine("Длительность не может быть больше 240");


            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var allServices = Lapitskaya_autoserviceEntities.GetContext().Service.ToList();
            allServices = allServices.Where(p => p.Title == _currentService.Title).ToList();

            if (allServices.Count == 0)
            {
                if (_currentService.ID == 0)
                    Lapitskaya_autoserviceEntities.GetContext().Service.Add(_currentService);
                try
                {
                    Lapitskaya_autoserviceEntities.GetContext().Service.Add(_currentService);
                    MessageBox.Show("информация сохранена");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
                MessageBox.Show("уже существует такая услуга");  

            if (_currentService.ID == 0)
                Lapitskaya_autoserviceEntities.GetContext().Service.Add(_currentService);
            try
            {
                Lapitskaya_autoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
