using ClosedXML.Excel;
using Microsoft.Win32;
using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFModernVerticalMenu.Pages
{
    public partial class ListChildren : Page
    {
        private string _connectionString = "Server=localhost;Port=5432;Database=db;User Id=postgres;Password=5422f8ab";

        public ListChildren()
        {
            InitializeComponent();
        }

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

        private void LoadData(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = $@"
                        SELECT 
                             g.group_name,
                             g.age_range,
                             COUNT(c.child_id) AS total_children,
                             {string.Join(", ", Enumerable.Range(1, (endDate - startDate).Days + 1)
                                  .Select(day => $@"
                                       SUM(CASE 
                                       WHEN a.date = '{startDate.AddDays(day - 1):yyyy-MM-dd}' AND a.was_present THEN 1 
                                       ELSE 0 
                                       END) AS day_{day}
                                   "))}
                        FROM 
                         attendance a
                        JOIN 
                         children c ON a.child_id = c.child_id
                        JOIN 
                         group_tbl g ON c.group_id = g.group_id
                        WHERE 
                         a.date BETWEEN @StartDate AND @EndDate
                        GROUP BY 
                          g.group_id, g.group_name, g.age_range
                        ORDER BY 
                         g.group_name
                        ";

                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataTable.Columns.Add("total_attendance", typeof(int));
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int totalAttendance = 0;

                        for (int day = 1; day <= (endDate - startDate).Days + 1; day++)
                        {
                            totalAttendance += row[$"day_{day}"] != DBNull.Value ? Convert.ToInt32(row[$"day_{day}"]) : 0;
                        }

                        row["total_attendance"] = totalAttendance;
                    }

                    AttendanceDataGrid.ItemsSource = dataTable.DefaultView;

                    AdjustColumnHeaders(dataTable);
                    AdjustColumnWidths();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void AdjustColumnHeaders(DataTable dataTable)
        {
            AttendanceDataGrid.Columns.Clear();

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Группа",
                Binding = new Binding("group_name")
            });

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Тариф",
                Binding = new Binding("age_range")
            });

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Кол-во детей",
                Binding = new Binding("total_children")
            });

            foreach (var column in dataTable.Columns.Cast<DataColumn>())
            {
                if (column.ColumnName.StartsWith("day_"))
                {
                    AttendanceDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = column.ColumnName.Replace("day_", ""),
                        Binding = new Binding(column.ColumnName)
                    });
                }
            }

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Итого",
                Binding = new Binding("total_attendance")
            });
        }

        private void AdjustColumnWidths()
        {
            foreach (var column in AttendanceDataGrid.Columns)
            {
                if (column.Header.ToString() == "Группа")
                {
                    column.Width = 130;
                }
                else if (column.Header.ToString() == "Тариф")
                {
                    column.Width = 80;
                }
                else if (column.Header.ToString() == "Кол-во детей")
                {
                    column.Width = 80;
                }
                else if (column.Header.ToString() == "Итого")
                {
                    column.Width = 50; 
                }
                else
                {
                    column.Width = 30;
                }
            }
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
