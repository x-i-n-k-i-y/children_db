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
                    c.child_id AS ""№"",
                    c.first_name AS ""Имя ребенка"",
                    p.full_name AS ""ФИО родителя"",
                    g.group_name AS ""Группа"",
                    gh.age_range AS ""Возрастная группа"",
                    t.rate AS ""Тариф"",
                    COALESCE(b.percentage, 0) AS ""Скидка, %""
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
                WHERE
                    @FromDate BETWEEN gh.date_from AND COALESCE(gh.date_to, '9999-12-31')
                    AND @FromDate BETWEEN t.date_from AND COALESCE(t.date_to, '9999-12-31')
                    AND @FromDate BETWEEN p.date_from AND COALESCE(p.date_to, '9999-12-31')
                GROUP BY
                    c.child_id, c.first_name, p.full_name, g.group_name, gh.age_range, t.rate, b.percentage";

                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FromDate", fromDate);

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Установка заголовков для DataGrid
                    DataGridResults.ItemsSource = dataTable.DefaultView;
                    AdjustColumnHeaders(dataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void AdjustColumnHeaders(DataTable dataTable)
        {
            DataGridResults.Columns.Clear();

            foreach (DataColumn column in dataTable.Columns)
            {
                DataGridResults.Columns.Add(new DataGridTextColumn
                {
                    Header = column.ColumnName, // Устанавливаем русское имя
                    Binding = new System.Windows.Data.Binding(column.ColumnName)
                });
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataGridResults.ItemsSource is not DataView dataView)
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
                        for (int i = 0; i < DataGridResults.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = DataGridResults.Columns[i].Header?.ToString() ?? string.Empty;
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