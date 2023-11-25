using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Demo_var_6.VMs;

namespace Demo_var_6
{
    public partial class Product : Window
    {
        private const string connectionString = "Data Source=KRINZHOTEKA\\KRINZHOTEKA;Initial Catalog=Demo_var_6;Integrated Security=True";
        public ObservableCollection<Products> productList { get; private set; } = new ObservableCollection<Products>();

        public Product()
        {
            InitializeComponent();
            DataContext = this;
            LoadProduct();
            ProductDataGrid.Sorting += DataGrid_Sorting;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text;
            ICollectionView view = CollectionViewSource.GetDefaultView(productList);

            if (string.IsNullOrWhiteSpace(searchText))
            {
                view.Filter = null; // Сброс фильтрации, если текстовое поле пусто
            }
            else
            {
                view.Filter = item =>
                {
                    Products product = (Products)item;
                    return product.ProductName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    // Используем IndexOf для поиска подстроки в ProductName.
                };
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            string columnName = e.Column.SortMemberPath;

            if (string.IsNullOrEmpty(columnName))
            {
                e.Handled = true;
                return;
            }

            var collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);

            if (collectionView != null)
            {
                if (collectionView.SortDescriptions.Count > 0 && collectionView.SortDescriptions[0].PropertyName == columnName)
                {
                    // Если уже сортировка по этому столбцу, меняем направление сортировки
                    var newDirection = collectionView.SortDescriptions[0].Direction == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;

                    collectionView.SortDescriptions.Clear();
                    collectionView.SortDescriptions.Add(new SortDescription(columnName, newDirection));
                }
                else
                {
                    // В противном случае, сортируем по выбранному столбцу по возрастанию
                    collectionView.SortDescriptions.Clear();
                    collectionView.SortDescriptions.Add(new SortDescription(columnName, ListSortDirection.Ascending));
                }
            }

            e.Handled = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    foreach (Products product in productList)
                    {
                        // Проверяем, не существует ли уже записи с таким артикулом
                        if (!IsProductExists(connection, product.Article))
                        {
                            // Если записи не существует, то добавляем ее
                            string insertQuery = "INSERT INTO Product (ProductName, Measure, Cost, MaxDiscount, Producer, " +
                                "Provider, Category, CurrentDiscount, StockQuantity, Description, ProductPhoto) " +
                                "VALUES (@ProductName, @Measure, @Cost, @MaxDiscount, @Producer, @Provider, @Category, " +
                                "@CurrentDiscount, @StockQuantity, @Description, @ProductPhoto)";

                            SqlCommand command = new SqlCommand(insertQuery, connection);

                            command.Parameters.AddWithValue("@ProductName", product.ProductName);
                            command.Parameters.AddWithValue("@Measure", product.Measure);
                            command.Parameters.AddWithValue("@Cost", product.Cost);
                            command.Parameters.AddWithValue("@MaxDiscount", product.MaxDiscount);
                            command.Parameters.AddWithValue("@Producer", product.Producer);
                            command.Parameters.AddWithValue("@Provider", product.Provider);
                            command.Parameters.AddWithValue("@Category", product.Category);
                            command.Parameters.AddWithValue("@CurrentDiscount", product.CurrentDiscount);
                            command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                            command.Parameters.AddWithValue("@Description", product.Description);
                            command.Parameters.AddWithValue("@ProductPhoto", product.ProductPhoto);

                            int rowsInserted = command.ExecuteNonQuery();
                            // Обработайте результат вставки, если необходимо
                        }
                    }

                    // Очистка коллекции после успешной вставки
                    productList.Clear();

                    // Загружаем данные заново
                    LoadProduct();

                    MessageBox.Show("Данные успешно добавлены.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private bool IsProductExists(SqlConnection connection, int article)
        {
            string checkQuery = "SELECT COUNT(*) FROM Product WHERE Article = @Article";
            SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
            checkCommand.Parameters.AddWithValue("@Article", article);

            int count = (int)checkCommand.ExecuteScalar();
            return count > 0;
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную строку в DataGrid
            Products selectedProduct = (Products)ProductDataGrid.SelectedItem;

            if (selectedProduct == null)
            {
                MessageBox.Show("Пожалуйста, выберите строку для редактирования.");
                return;
            }

            // Получаем измененные значения из выбранной строки
            string newProductName = selectedProduct.ProductName;
            string newMeasure = selectedProduct.Measure;
            int newCost = selectedProduct.Cost;
            int newMaxDiscount = selectedProduct.MaxDiscount;
            string newProducer = selectedProduct.Producer;
            string newProvider = selectedProduct.Provider;
            string newCategory = selectedProduct.Category;
            int newCurrentDiscount = selectedProduct.CurrentDiscount;
            int newStockQuantity = selectedProduct.StockQuantity;
            string newDescription = selectedProduct.Description;
            string newProductPhoto = selectedProduct.ProductPhoto;

            // Обновляем данные в базе данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string updateQuery = "UPDATE Product " +
                        "SET ProductName = @ProductName, Measure = @Measure, Cost = @Cost, MaxDiscount = @MaxDiscount, " +
                        "Producer = @Producer, Provider = @Provider, Category = @Category, CurrentDiscount = @CurrentDiscount, " +
                        "StockQuantity = @StockQuantity, Description = @Description, ProductPhoto = @ProductPhoto " +
                        "WHERE Article = @Article";

                    SqlCommand command = new SqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@ProductName", newProductName);
                    command.Parameters.AddWithValue("@Measure", newMeasure);
                    command.Parameters.AddWithValue("@Cost", newCost);
                    command.Parameters.AddWithValue("@MaxDiscount", newMaxDiscount);
                    command.Parameters.AddWithValue("@Producer", newProducer);
                    command.Parameters.AddWithValue("@Provider", newProvider);
                    command.Parameters.AddWithValue("@Category", newCategory);
                    command.Parameters.AddWithValue("@CurrentDiscount", newCurrentDiscount);
                    command.Parameters.AddWithValue("@StockQuantity", newStockQuantity);
                    command.Parameters.AddWithValue("@Description", newDescription);
                    command.Parameters.AddWithValue("@ProductPhoto", newProductPhoto);
                    command.Parameters.AddWithValue("@Article", selectedProduct.Article);

                    int rowsUpdated = command.ExecuteNonQuery();

                    if (rowsUpdated > 0)
                    {
                        MessageBox.Show("Изменения успешно сохранены.");
                        // Можете обновить DataGrid после успешного сохранения данных
                        LoadProduct();
                    }
                    else
                    {
                        MessageBox.Show("Изменения не были сохранены. Проверьте данные и попробуйте еще раз.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при сохранении изменений: " + ex.Message);
                }
            }
        }

        private void LoadProduct()
        {
            productList.Clear(); // Очищаем коллекцию перед загрузкой данных

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM Product", connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int article = reader.GetInt32(0);
                        string productName = reader.GetString(1);
                        string measure = reader.GetString(2);
                        int cost = reader.GetInt32(3);
                        int maxDiscount = reader.GetInt32(4);
                        string producer = reader.GetString(5);
                        string provider = reader.GetString(6);
                        string category = reader.GetString(7);
                        int currentDiscount = reader.GetInt32(8);
                        int stockQuantity = reader.GetInt32(9);
                        string description = reader.GetString(10);
                        string productPhoto = reader.GetString(11);

                        Products product = new Products
                        {
                            Article = article,
                            ProductName = productName,
                            Measure = measure,
                            Cost = cost,
                            MaxDiscount = maxDiscount,
                            Producer = producer,
                            Provider = provider,
                            Category = category,
                            CurrentDiscount = currentDiscount,
                            StockQuantity = stockQuantity,
                            Description = description,
                            ProductPhoto = productPhoto
                        };
                        productList.Add(product);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }
    }
}
