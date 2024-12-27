using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace WPFModernVerticalMenu.Pages
{
    public partial class Payments : Page
    {
        private DataTable _dataTable;
        private string _connectionString = "Server=localhost;Port=5432;Database=db;User Id=postgres;Password=5422f8ab";

        public Payments()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM payments ORDER BY payment_id";
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                    _dataTable = new DataTable();
                    adapter.Fill(_dataTable);
                    DataGridTable.ItemsSource = _dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }


        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM payments";
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                    NpgsqlCommandBuilder builder = new NpgsqlCommandBuilder(adapter);

                    adapter.Update(_dataTable);
                    MessageBox.Show("Changes saved successfully!");

                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}");
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.Header.ToString() == "payment_id")
            {
                e.Handled = true;

                DataView view = (DataView)DataGridTable.ItemsSource;
                view.Sort = "payment_id ASC";
                DataGridTable.ItemsSource = view;
            }
        }


    }
}