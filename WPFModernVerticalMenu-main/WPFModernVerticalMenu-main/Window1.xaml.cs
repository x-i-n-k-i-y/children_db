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

namespace WPFModernVerticalMenu
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Frame _frameContainer;
        public Window1(Frame frameContainer)
        {
            InitializeComponent();
            _frameContainer = frameContainer;
        }

        // SecondaryWindow.xaml.cs
        private void btnGroups_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Home.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnChildren_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Children.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnParents_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Parents.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnTariffs_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Tariffs.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnAttendance_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Attendance.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnBenefits_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Benefits.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnPayments_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Payments.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnChild_history_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Child_history.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnGroup_history_Click(object sender, RoutedEventArgs e)
        {
            _frameContainer.Navigate(new Uri("Pages/Group_history.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
