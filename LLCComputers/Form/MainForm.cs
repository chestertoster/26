using LLCComputers.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LLCComputers
{
    public partial class MainForm : Form
    {
        private readonly LLCComputerEntities _dbContext;

        private bool _priceSort = true;

        private readonly string[] _comboBoxData = new string[]
        {
            "Все диапазоны",
            "0-9.99%",
            "10-14.99%",
            "15+%"
        };

        public MainForm()
        {
            InitializeComponent();

            _dbContext = new LLCComputerEntities();

            comboBoxFilterDiscount.DataSource = _comboBoxData;

            InitializeDataGridView();
            UpdateDataInfo();
        }

        private void InitializeDataGridView()
        {
            dataGridView.DataSource = _dbContext
                .Products
                .ToList();

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.RowHeadersVisible = false;
            dataGridView.AllowUserToResizeRows = false;

            dataGridView.Columns["Manufacturer"].Visible = false;
            dataGridView.Columns["ProductGroup"].Visible = false;

            dataGridView.Columns["ProductId"].HeaderText = "Артикул";
            dataGridView.Columns["ProductName"].HeaderText = "Название";
            dataGridView.Columns["Description"].HeaderText = "Описание";
            dataGridView.Columns["ManufacturerId"].HeaderText = "Производитель";
            dataGridView.Columns["ProductGroupId"].HeaderText = "Группа товаров";
            dataGridView.Columns["Price"].HeaderText = "Цена";
            dataGridView.Columns["Discount"].HeaderText = "Скидка";
        }

        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn clickedColumn = dataGridView.Columns[e.ColumnIndex];

            // Проверка, что колонка является той, по которой хотите сортировать
            if (clickedColumn == dataGridView.Columns["Price"])
            {
                SortDataByColumn(ref _priceSort);
            }
        }

        private void SortDataByColumn(ref bool sort)
        {
            var products = new List<Product>();

            // Определение направления сортировки
            if (sort)
            {
                products = _dbContext
                    .Products
                    .OrderBy(p => p.Price)
                    .ToList();
            }
            else
            {
                products = _dbContext
                    .Products
                    .OrderByDescending(p => p.Price)
                    .ToList();
            }

            sort = !sort;

            dataGridView.DataSource = products; // Обновление DataGridView с отсортированными данными
            UpdateDataInfo(); // Обновление информации о количестве данных
        }

        private void UpdateDataInfo()
        {
            int displayedRows = dataGridView.Rows.Count;

            int totalRows = _dbContext
                .Products
                .ToList()
                .Count;

            labelDataInfo.Text = $"{displayedRows} из {totalRows}";
        }

        private void ComboBoxFilterDiscount_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = comboBoxFilterDiscount.Text;

            switch (text)
            {
                case "Все диапазоны":
                    dataGridView.DataSource = _dbContext.Products.ToArray();
                    break;

                case "0-9.99%":
                    dataGridView.DataSource = _dbContext.Products.Where(p => p.Discount >= 0 && p.Discount < 10).ToArray();
                    break;

                case "10-14.99%":
                    dataGridView.DataSource = _dbContext.Products.Where(p => p.Discount >= 10 && p.Discount < 15).ToArray();
                    break;

                case "15+%":
                    dataGridView.DataSource = _dbContext.Products.Where(p => p.Discount >= 15).ToArray();
                    break;
            }
        }

        // Изменение цвета скидки
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].Name != "Discount")
            {
                return;
            }

            object cellValue = e.Value;

            if (cellValue is null)
            {
                return;
            }

            if (!decimal.TryParse(cellValue.ToString(), out decimal value))
            {
                return;
            }

            if (value > 5) // Замените это условие на нужное вам
            {
                e.CellStyle.BackColor = Color.FromArgb(0x7F, 0xFF, 0x00);
                return;
            }

            e.CellStyle.BackColor = dataGridView.DefaultCellStyle.BackColor;
        }

        private void ButtonSearch_Click_1(object sender, EventArgs e)
        {
            string searchText = textBoxSearch.Text.ToLower(); // Получаем текст поиска и приводим к нижнему регистру для сравнения

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                // Выполните поиск в списке продуктов по имени продукта
                var searchResults = _dbContext
                    .Products
                    .Where(p => p.ProductName.ToLower().Contains(searchText)).ToList();

                // Обновление DataGridView с результатами поиска
                dataGridView.DataSource = searchResults;
            }
            else
            {
                // Если текст поиска пуст, отобразите все продукты
                dataGridView.DataSource = _dbContext
                .Products
                .ToList();
            }

            UpdateDataInfo();
        }

        private void ButtonReset_Click_1(object sender, EventArgs e)
        {
            textBoxSearch.Text = string.Empty; // Очистить содержимое текстового поля поиска

            // Отобразить полный список продуктов в DataGridView
            dataGridView.DataSource = _dbContext
                .Products
                .ToList();

            UpdateDataInfo(); // Обновить информацию о количестве данных
        }
    }
}