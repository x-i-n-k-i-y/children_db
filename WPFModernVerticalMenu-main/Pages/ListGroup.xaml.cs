using ClosedXML.Excel;
using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Логика взаимодействия для ListGroup.xaml
    /// </summary>
    public partial class ListGroup : Page
    {
        private string _connectionString = "Server=localhost;Port=5432;Database=db;User Id=postgres;Password=5422f8ab";

        public ListGroup()
        {
            InitializeComponent();
        }

        // Метод для загрузки данных за выбранный месяц и год
        private void LoadAttendance(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedMonth = (ComboBoxItem)MonthSelector.SelectedItem;
            ComboBoxItem selectedYear = (ComboBoxItem)YearSelector.SelectedItem;

            if (selectedMonth == null || selectedYear == null)
            {
                MessageBox.Show("Выберите месяц и год!");
                return;
            }

            string month = selectedMonth.Tag.ToString();
            int year = int.Parse(selectedYear.Tag.ToString());

            DateTime startDate = new DateTime(year, int.Parse(month), 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            LoadData(startDate, endDate);
        }

        // Метод для загрузки данных из базы данных
        private void LoadData(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT
                            g.group_name,
                            gh.age_range,
                            c.child_id,
                            c.first_name
                        FROM
                            children c
                        JOIN
                            group_tbl g ON c.group_id = g.group_id
                        JOIN
                            group_history gh ON c.group_id = gh.group_id
                        WHERE
                            gh.date_from <= @EndDate AND (gh.date_to IS NULL OR gh.date_to >= @StartDate)
                        ORDER BY
                            g.group_name, c.child_id, c.first_name;
                    ";

                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    AttendanceDataGrid.ItemsSource = dataTable.DefaultView;
                    AdjustColumnHeaders(dataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        // Метод для настройки заголовков столбцов
        private void AdjustColumnHeaders(DataTable dataTable)
        {
            AttendanceDataGrid.Columns.Clear();

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Группа",
                Binding = new System.Windows.Data.Binding("group_name")
            });

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Возрастной диапазон",
                Binding = new System.Windows.Data.Binding("age_range")
            });

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID ребенка",
                Binding = new System.Windows.Data.Binding("child_id"),
                SortMemberPath = "child_id"
            });

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Имя ребенка",
                Binding = new System.Windows.Data.Binding("first_name"),
                SortMemberPath = "first_name"
            });
        }
        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AttendanceDataGrid.ItemsSource is not DataView dataView)
                {
                    MessageBox.Show("Нет данных для экспорта.");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = "Табель.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Табель");

                        // Добавляем заголовки
                        for (int i = 0; i < AttendanceDataGrid.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = AttendanceDataGrid.Columns[i].Header?.ToString() ?? string.Empty;
                        }

                        // Добавляем данные
                        var dataTable = dataView.ToTable();
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < dataTable.Columns.Count; j++)
                            {
                                object cellValue = dataTable.Rows[i][j];

                                // Явное приведение типов для записи значения
                                if (cellValue is int intValue)
                                {
                                    worksheet.Cell(i + 2, j + 1).Value = intValue;
                                }
                                else if (cellValue is double doubleValue)
                                {
                                    worksheet.Cell(i + 2, j + 1).Value = doubleValue;
                                }
                                else if (cellValue is decimal decimalValue)
                                {
                                    worksheet.Cell(i + 2, j + 1).Value = (double)decimalValue; // Преобразуем decimal в double
                                }
                                else if (cellValue is DateTime dateTimeValue)
                                {
                                    worksheet.Cell(i + 2, j + 1).Value = dateTimeValue;
                                }
                                else
                                {
                                    worksheet.Cell(i + 2, j + 1).Value = cellValue?.ToString() ?? string.Empty; // Преобразуем в строку
                                }
                            }
                        }

                        // Автоматическое изменение ширины столбцов
                        worksheet.Columns().AdjustToContents();

                        // Сохранение файла
                        using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                        {
                            workbook.SaveAs(stream);
                        }
                    }

                    MessageBox.Show("Данные успешно сохранены в Excel.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }
    }
}
