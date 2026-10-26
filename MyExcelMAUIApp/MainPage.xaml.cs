using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MyExcelMAUIApp
{
    public partial class MainPage : ContentPage
    {
        private const int InitialCountColumn = 20;
        private const int InitialCountRow = 50;
        private const int AlphabetLength = 26;
        private const int AsciiCodeA = 65;

        // Модель даних: зберігає формули (або введений текст) та доступ до UI
        private Dictionary<string, string> _cellFormulas = new Dictionary<string, string>();
        private Dictionary<string, Entry> _uiCells = new Dictionary<string, Entry>();

        public MainPage()
        {
            InitializeComponent();
            CreateGrid();
        }

        private void CreateGrid()
        {
            AddColumnsAndColumnLabels();
            AddRowsAndCellEntries();
        }

        private void AddColumnsAndColumnLabels()
        {
            for (int col = 0; col < InitialCountColumn + 1; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                if (col > 0)
                {
                    var label = new Label
                    {
                        Text = GetColumnName(col),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, col);
                    grid.Children.Add(label);
                }
            }
        }

        private void AddRowsAndCellEntries()
        {
            grid.RowDefinitions.Add(new RowDefinition());

            for (int row = 0; row < InitialCountRow; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition());

                var label = new Label
                {
                    Text = (row + 1).ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                Grid.SetRow(label, row + 1);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);

                for (int col = 0; col < InitialCountColumn; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    string cellName = $"{GetColumnName(col + 1)}{row + 1}";
                    _uiCells[cellName] = entry;

                    entry.Focused += Entry_Focused;
                    entry.Unfocused += Entry_Unfocused;

                    Grid.SetRow(entry, row + 1);
                    Grid.SetColumn(entry, col + 1);
                    grid.Children.Add(entry);
                }
            }
        }

        public static string GetColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = string.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % AlphabetLength;
                columnName = Convert.ToChar(AsciiCodeA + modulo) + columnName;
                dividend = (dividend - modulo) / AlphabetLength;
            }

            return columnName;
        }

        private string GetCellName(Entry entry)
        {
            int row = Grid.GetRow(entry);
            int col = Grid.GetColumn(entry);
            return $"{GetColumnName(col)}{row}";
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            string cellName = GetCellName(entry);

            // Коли клітинка у фокусі, показуємо її реальну формулу
            if (_cellFormulas.ContainsKey(cellName))
            {
                entry.Text = _cellFormulas[cellName];
            }

            textInput.Text = entry.Text;
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            string cellName = GetCellName(entry);

            // Зберігаємо введену формулу у словник і перераховуємо таблицю
            _cellFormulas[cellName] = entry.Text;
            UpdateGridValues();
        }

        private void CalculateButton_Clicked(object sender, EventArgs e)
        {
            UpdateGridValues();
        }

        private void UpdateGridValues()
        {
            var evaluator = new Evaluator(_cellFormulas);

            foreach (var kvp in _cellFormulas)
            {
                if (kvp.Value.StartsWith("="))
                {
                    _uiCells[kvp.Key].Text = evaluator.Evaluate(kvp.Value.Substring(1));
                }
                else
                {
                    _uiCells[kvp.Key].Text = kvp.Value;
                }
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            string json = JsonSerializer.Serialize(_cellFormulas);
            string path = Path.Combine(FileSystem.AppDataDirectory, "table.json");
            File.WriteAllText(path, json);
            await DisplayAlert("Збережено", "Таблицю успішно збережено у пам'ять пристрою.", "OK");
        }

        private async void ReadButton_Clicked(object sender, EventArgs e)
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "table.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                _cellFormulas = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                UpdateGridValues();
                await DisplayAlert("Завантажено", "Дані відновлено.", "OK");
            }
            else
            {
                await DisplayAlert("Помилка", "Збережений файл не знайдено.", "OK");
            }
        }

        private async void ExitButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти?", "Так", "Ні");
            if (answer)
            {
                System.Environment.Exit(0);
            }
        }

        private async void HelpButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Довідка", "Лабораторна робота 1. Студентки Марії Рокочої\n\nДля розрахунків введіть '=' перед виразом (наприклад, =A1+B2).", "OK");
        }

        private void DeleteRowButton_Clicked(object sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 1)
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;
                var elementsToRemove = grid.Children.Where(c => Grid.GetRow((BindableObject)c) == lastRowIndex).ToList();

                foreach (var child in elementsToRemove)
                {
                    if (child is Entry entry)
                    {
                        string cellName = GetCellName(entry);
                        _uiCells.Remove(cellName);
                        _cellFormulas.Remove(cellName);
                    }
                    grid.Children.Remove(child);
                }

                grid.RowDefinitions.RemoveAt(lastRowIndex);
            }
        }

        private void DeleteColumnButton_Clicked(object sender, EventArgs e)
        {
            if (grid.ColumnDefinitions.Count > 1)
            {
                int lastColumnIndex = grid.ColumnDefinitions.Count - 1;
                var elementsToRemove = grid.Children.Where(c => Grid.GetColumn((BindableObject)c) == lastColumnIndex).ToList();

                foreach (var child in elementsToRemove)
                {
                    if (child is Entry entry)
                    {
                        string cellName = GetCellName(entry);
                        _uiCells.Remove(cellName);
                        _cellFormulas.Remove(cellName);
                    }
                    grid.Children.Remove(child);
                }

                grid.ColumnDefinitions.RemoveAt(lastColumnIndex);
            }
        }

        private void AddRowButton_Clicked(object sender, EventArgs e)
        {
            int newRow = grid.RowDefinitions.Count;
            grid.RowDefinitions.Add(new RowDefinition());

            var label = new Label
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            int currentColumns = grid.ColumnDefinitions.Count - 1;

            for (int col = 0; col < currentColumns; col++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                string cellName = $"{GetColumnName(col + 1)}{newRow}";
                _uiCells[cellName] = entry;

                entry.Focused += Entry_Focused;
                entry.Unfocused += Entry_Unfocused;

                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);
            }
        }

        private void AddColumnButton_Clicked(object sender, EventArgs e)
        {
            int newColumn = grid.ColumnDefinitions.Count;
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            var label = new Label
            {
                Text = GetColumnName(newColumn),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Grid.SetRow(label, 0);
            Grid.SetColumn(label, newColumn);
            grid.Children.Add(label);

            int currentRows = grid.RowDefinitions.Count - 1;

            for (int row = 0; row < currentRows; row++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                string cellName = $"{GetColumnName(newColumn)}{row + 1}";
                _uiCells[cellName] = entry;

                entry.Focused += Entry_Focused;
                entry.Unfocused += Entry_Unfocused;

                Grid.SetRow(entry, row + 1);
                Grid.SetColumn(entry, newColumn);
                grid.Children.Add(entry);
            }
        }
    }
}