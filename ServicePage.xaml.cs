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
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage;

        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            if (CountRecords % 10 > 0)
                CountPage = CountRecords / 10 + 1;
            else
                CountPage = CountPage / 10;

            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage*10+10 < CountRecords ? CurrentPage*10+10 : CountRecords;
                    for(int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                            Ifupdate = false;
                        break;
                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                            Ifupdate = false;
                        break;
                }
            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBALLRecords.Text = " из " + CountRecords.ToString();
                
                ServiceListView.ItemsSource = CurrentPageList;
                ServiceListView.Items.Refresh();
            }
        }
        
        public ServicePage()
        {
            InitializeComponent();

            var currrentService = Lapitskaya_autoserviceEntities.GetContext().Service.ToList();

            ServiceListView.ItemsSource = currrentService;

            ComboType.SelectedIndex = 0;

            UpdateService();
        }
        private void UpdateService()
        {
            var currentService = Lapitskaya_autoserviceEntities.GetContext().Service.ToList();

            if (ComboType.SelectedIndex == 0)
            {
                currentService = currentService.Where(p => (Convert.ToInt32(p.DiscountInt) >= 0 && Convert.ToInt32(p.DiscountInt) <= 100)).ToList();

            }
            if (ComboType.SelectedIndex == 1)
            {
                currentService = currentService.Where(p => (Convert.ToInt32(p.DiscountInt) >= 0 && Convert.ToInt32(p.DiscountInt) < 5)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentService = currentService.Where(p => (Convert.ToInt32(p.DiscountInt) >= 5 && Convert.ToInt32(p.DiscountInt) < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentService = currentService.Where(p => (Convert.ToInt32(p.DiscountInt) >= 15 && Convert.ToInt32(p.DiscountInt) < 30)).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentService = currentService.Where(p => (Convert.ToInt32(p.DiscountInt) >= 30 && Convert.ToInt32(p.DiscountInt) < 70)).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentService = currentService.Where(p => (Convert.ToInt32(p.DiscountInt) >= 70 && Convert.ToInt32(p.DiscountInt) <= 100)).ToList();
            }

            currentService = currentService.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ServiceListView.ItemsSource = currentService.ToList();

            if (RButtonDown.IsChecked.Value)
            {
                ServiceListView.ItemsSource = currentService.OrderByDescending(p => p.Cost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                ServiceListView.ItemsSource = currentService.OrderBy(p => p.Cost).ToList();
            }

            if (RButtonDown.IsChecked.Value)
                currentService = currentService.OrderByDescending(p => p.Cost).ToList();

            if(RButtonUp.IsChecked.Value)
                currentService = currentService.OrderBy(p =>p.Cost).ToList();
            ServiceListView.ItemsSource = currentService;
            TableList = currentService;
            ChangePage(0, 0);
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateService();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateService();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateService();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateService();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Visibility == Visibility.Visible)
            {
                Lapitskaya_autoserviceEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Lapitskaya_autoserviceEntities.GetContext().Service.ToList();
            }
            UpdateService();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentService = (sender as Button).DataContext as Service;
            
            var currentClientService = Lapitskaya_autoserviceEntities.GetContext().ClientService.ToList();
            currentClientService = currentClientService.Where(p => p.ServiceID == currentService.ID).ToList();

            if (currentClientService.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, тк как существует запись на эту услугу");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Lapitskaya_autoserviceEntities.GetContext().Service.Remove(currentService);
                        Lapitskaya_autoserviceEntities.GetContext().SaveChanges();

                        ServiceListView.ItemsSource = Lapitskaya_autoserviceEntities.GetContext().Service.ToList();

                        UpdateService();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2,null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString())-1);
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new SignUpPage((sender as Button).DataContext as Service));
        }
    }
}

