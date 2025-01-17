using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace WPFModernVerticalMenu
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        // Start: MenuLeft PopupButton //
        private void btnHome_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnHome;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Таблицы";
            }
        }

        private void btnHome_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnDashboard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnDashboard;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Информация о детях на определенную дату";
            }
        }

        private void btnDashboard_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnProducts_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnProducts;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Табель по группе";
            }
        }

        private void btnProducts_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnProductStock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnProductStock;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Табель по саду";
            }
        }

        private void btnProductStock_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnOrderList_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnOrderList;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Список детей, сгруппированных в определенном месяце";
            }
        }

        private void btnOrderList_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            foreach(Window w in App.Current.Windows)
                w.Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            Window1 newWindow = new Window1(fContainer);
            newWindow.Show();
        }



        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/Dashboard.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/Products.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnListChildren_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/ListChildren.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnListGroup_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/ListGroup.xaml", UriKind.RelativeOrAbsolute));
        }

        string connectionString = "Server=localhost;Port=5432;Database=db;User Id = postgres; password=5422f8ab";

        private void SqlConnectionReader()
        {
            NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString);
            sqlConnection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = sqlConnection;
            sqlConnection.Close();
        }



    }
}
