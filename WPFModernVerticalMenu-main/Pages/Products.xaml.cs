using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.IO;


namespace WPFModernVerticalMenu.Pages
{
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

                    string query = $@"
                        SELECT 
                            c.child_id,
                            c.first_name,
                            {string.Join(", ", Enumerable.Range(1, (endDate - startDate).Days + 1)
                                .Select(day => $@"
                                    MAX(CASE 
                                        WHEN a.date = '{startDate.AddDays(day - 1):yyyy-MM-dd}' THEN 
                                            CASE WHEN a.was_present THEN 'Б' ELSE 'Н' END 
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
                            AND g.group_id = @GroupId
                        GROUP BY 
                            c.child_id, c.first_name
                        ORDER BY 
                            c.child_id";

                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@GroupId", groupId);

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Добавляем колонку "Итого дней"
                    dataTable.Columns.Add("total_days", typeof(int));
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int totalDays = 0;

                        for (int day = 1; day <= (endDate - startDate).Days + 1; day++)
                        {
                            if (row[$"day_{day}"] != DBNull.Value && row[$"day_{day}"].ToString() == "Б")
                            {
                                totalDays++;
                            }
                        }

                        row["total_days"] = totalDays;
                    }

                    // Привязка данных к DataGrid
                    AttendanceDataGrid.ItemsSource = dataTable.DefaultView;

                    // Настройка заголовков
                    AdjustColumnHeaders();
                    AdjustColumnWidths();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void AdjustColumnHeaders()
        {
            AttendanceDataGrid.Columns.Clear();

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "№",
                Binding = new System.Windows.Data.Binding("child_id")
            });

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Ф.И. ребенка",
                Binding = new System.Windows.Data.Binding("first_name")
            });

            foreach (var column in AttendanceDataGrid.ItemsSource.Cast<DataRowView>().First().Row.Table.Columns.Cast<DataColumn>())
            {
                if (column.ColumnName.StartsWith("day_"))
                {
                    AttendanceDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = column.ColumnName.Replace("day_", ""),
                        Binding = new System.Windows.Data.Binding(column.ColumnName)
                    });
                }
            }

            AttendanceDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Итого",
                Binding = new System.Windows.Data.Binding("total_days")
            });
        }

        private void AdjustColumnWidths()
        {
            foreach (var column in AttendanceDataGrid.Columns)
            {
                if (column.Header.ToString() == "Ф.И. ребенка")
                {
                    column.Width = 130; // Устанавливаем фиксированную ширину для колонки с именами
                }
                else if (column.Header.ToString() == "Итого")
                {
                    column.Width = 50; // Устанавливаем фиксированную ширину для итоговой колонки
                }
                else
                {
                    column.Width = 30; // Устанавливаем ширину для столбцов с днями
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
