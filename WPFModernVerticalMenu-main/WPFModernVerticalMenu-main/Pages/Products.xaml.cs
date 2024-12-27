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
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Products : Page
    {
        private string _connectionString = "Server=localhost;Port=5432;Database=db;User Id=postgres;Password=5422f8ab";

        public Products()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT group_id, group_name FROM group_tbl";
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    GroupSelector.ItemsSource = dataTable.DefaultView;
                    GroupSelector.DisplayMemberPath = "group_name";
                    GroupSelector.SelectedValuePath = "group_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}");
            }
        }

        private void LoadAttendance_Click(object sender, RoutedEventArgs e)
        {
            // Получение выбранного месяца, года и группы
            ComboBoxItem selectedMonth = (ComboBoxItem)MonthSelector.SelectedItem;
            ComboBoxItem selectedYear = (ComboBoxItem)YearSelector.SelectedItem;
            DataRowView selectedGroup = (DataRowView)GroupSelector.SelectedItem;

            if (selectedMonth == null || selectedYear == null || selectedGroup == null)
            {
                MessageBox.Show("Выберите месяц, год и группу!");
                return;
            }

            string month = selectedMonth.Tag.ToString();
            int year = int.Parse(selectedYear.Tag.ToString());
            int groupId = int.Parse(selectedGroup["group_id"].ToString());

            // Вычисление начала и конца месяца
            DateTime startDate = new DateTime(year, int.Parse(month), 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            LoadData(startDate, endDate, groupId);
        }

        private void LoadData(DateTime startDate, DateTime endDate, int groupId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT
                            c.child_id,
                            c.first_name AS full_name,
                            g.group_name,
                            a.date,
                            a.was_present
                        FROM
                            attendance a
                        JOIN
                            children c ON a.child_id = c.child_id
                        JOIN
                            group_tbl g ON c.group_id = g.group_id
                        WHERE
                            a.date BETWEEN @StartDate AND @EndDate
                            AND g.group_id = @GroupId
                        ORDER BY
                            c.child_id, a.date";

                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@GroupId", groupId);

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Transform data to match the required format
                    var transformedTable = TransformData(dataTable, startDate, endDate);
                    AttendanceDataGrid.ItemsSource = transformedTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private DataTable TransformData(DataTable dataTable, DateTime startDate, DateTime endDate)
        {
            DataTable transformedTable = new DataTable();
            transformedTable.Columns.Add("№ п/п");
            transformedTable.Columns.Add("Ф.И. ребенка");

            int daysInMonth = (endDate - startDate).Days + 1;
            for (int i = 1; i <= daysInMonth; i++)
            {
                transformedTable.Columns.Add(i.ToString());
            }

            transformedTable.Columns.Add("Дни по месяцу");
            transformedTable.Columns.Add("Табель номер");
            transformedTable.Columns.Add("Дней по опят");

            var groupedData = dataTable.AsEnumerable()
                .GroupBy(row => new { ChildId = row.Field<int>("child_id"), FullName = row.Field<string>("full_name") })
                .Select((group, index) => new
                {
                    Index = index + 1,
                    FullName = group.Key.FullName,
                    Days = group.ToList().Select(row => new { Date = row.Field<DateTime>("date"), WasPresent = row.Field<bool>("was_present") }).ToList()
                });

            foreach (var child in groupedData)
            {
                DataRow newRow = transformedTable.NewRow();
                newRow["№ п/п"] = child.Index;
                newRow["Ф.И. ребенка"] = child.FullName;

                foreach (var day in child.Days)
                {
                    int dayOfMonth = day.Date.Day;
                    newRow[dayOfMonth.ToString()] = day.WasPresent ? "Б" : "Н";
                }

                newRow["Дни по месяцу"] = child.Days.Count(day => day.WasPresent);
                newRow["Табель номер"] = "Табель";
                newRow["Дней по опят"] = child.Days.Count(day => !day.WasPresent);

                transformedTable.Rows.Add(newRow);
            }

            return transformedTable;
        }
    }
}
