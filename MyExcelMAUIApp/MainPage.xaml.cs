using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyExcelMAUIApp
{
    public partial class MainPage : ContentPage
    {
        // Константи для ініціалізації
        private const int InitialCountColumn = 20;
        private const int InitialCountRow = 50;
        private const int AlphabetLength = 26;
        private const int AsciiCodeA = 65;

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

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var content = entry.Text;
        }

        private void CalculateButton_Clicked(object sender, EventArgs e) { }
        private void SaveButton_Clicked(object sender, EventArgs e) { }
        private void ReadButton_Clicked(object sender, EventArgs e) { }

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
            await DisplayAlert("Довідка", "Лабораторна робота 1. Студентки Марії Рокочої", "OK");
        }

        // Логіка для додавання та видалення рядків і стовпців

        private void DeleteRowButton_Clicked(object sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 1) // Залишаємо хоча б рядок з літерами
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;

                // Знаходимо всі елементи, які належать до останнього рядка, і видаляємо їх
                var elementsToRemove = grid.Children.Where(c => Grid.GetRow((BindableObject)c) == lastRowIndex).ToList();
                foreach (var child in elementsToRemove)
                {
                    grid.Children.Remove(child);
                }

                grid.RowDefinitions.RemoveAt(lastRowIndex);
            }
        }

        private void DeleteColumnButton_Clicked(object sender, EventArgs e)
        {
            if (grid.ColumnDefinitions.Count > 1) // Залишаємо хоча б стовпець з цифрами
            {
                int lastColumnIndex = grid.ColumnDefinitions.Count - 1;

                // Знаходимо всі елементи, які належать до останнього стовпця, і видаляємо їх
                var elementsToRemove = grid.Children.Where(c => Grid.GetColumn((BindableObject)c) == lastColumnIndex).ToList();
                foreach (var child in elementsToRemove)
                {
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

            // Використовуємо ФАКТИЧНУ кількість стовпців, а не константу
            int currentColumns = grid.ColumnDefinitions.Count - 1;

            for (int col = 0; col < currentColumns; col++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

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

            // Використовуємо ФАКТИЧНУ кількість рядків, а не константу
            int currentRows = grid.RowDefinitions.Count - 1;

            for (int row = 0; row < currentRows; row++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, row + 1);
                Grid.SetColumn(entry, newColumn);
                grid.Children.Add(entry);
            }
        }
    }
}
