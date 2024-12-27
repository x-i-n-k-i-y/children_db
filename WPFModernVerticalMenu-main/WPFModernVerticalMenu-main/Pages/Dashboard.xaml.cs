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
    public partial class Dashboard : Page
    {
        private string _connectionString = "Server=localhost;Port=5432;Database=db;User Id=postgres;Password=5422f8ab";

        public Dashboard()
        {
            InitializeComponent();
        }

        private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            DateTime fromDate = DatePickerFromDate.SelectedDate ?? DateTime.Today;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT
                            c.child_id,
                            c.first_name,
                            p.full_name AS parent_name,
                            g.group_name,
                            gh.age_range,
                            t.rate AS current_rate,
                            COALESCE(b.percentage, 0) AS benefit_percentage,
                            a.date AS attendance_date,
                            a.was_present
                        FROM
                            children c
                        JOIN
                            parents p ON c.child_id = p.child_id
                        JOIN
                            child_history ch ON c.child_id = ch.child_id
                        JOIN
                            group_history gh ON ch.group_id = gh.group_id
                        JOIN
                            group_tbl g ON gh.group_id = g.group_id
                        JOIN
                            tariffs t ON gh.group_id = t.group_id
                        LEFT JOIN
                            benefits b ON c.child_id = b.child_id AND @FromDate BETWEEN b.date_from AND COALESCE(b.date_to, '9999-12-31')
                        LEFT JOIN
                            attendance a ON c.child_id = a.child_id AND a.date = @FromDate
                        WHERE
                            @FromDate BETWEEN gh.date_from AND COALESCE(gh.date_to, '9999-12-31')
                            AND @FromDate BETWEEN t.date_from AND COALESCE(t.date_to, '9999-12-31')
                            AND @FromDate BETWEEN p.date_from AND COALESCE(p.date_to, '9999-12-31')
                        GROUP BY
                            c.child_id, c.first_name, p.full_name, g.group_name, gh.age_range, t.rate, b.percentage, a.date, a.was_present";

                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FromDate", fromDate);

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    DataGridResults.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}