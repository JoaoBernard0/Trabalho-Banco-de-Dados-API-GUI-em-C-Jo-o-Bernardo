using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace InventoryGui
{
    // DTO simples usados pelo GUI (copie se já não existir InventoryGui/Models)
    public class CategoryDto { public int Id { get; set; } public string Name { get; set; } = null!; }
    public class ProductDto { public int Id { get; set; } public string Name { get; set; } = null!; public decimal Price { get; set; } public int CategoryId { get; set; } public string? CategoryName { get; set; } }

    public partial class ProductEditWindow : Window
    {
        private readonly HttpClient _http;
        private readonly int? _productId;

        public ProductEditWindow(HttpClient httpClient, int? productId = null)
        {
            InitializeComponent();
            _http = httpClient;
            _productId = productId;
            _ = LoadCategoriesAsync();

            if (_productId.HasValue)
                _ = LoadProductAsync(_productId.Value);
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var cats = await _http.GetFromJsonAsync<List<CategoryDto>>("api/categories");
                CmbCategories.ItemsSource = cats;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar categorias: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProductAsync(int id)
        {
            try
            {
                var prod = await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}");
                if (prod == null)
                {
                    MessageBox.Show("Produto não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                TxtName.Text = prod.Name;
                TxtPrice.Text = prod.Price.ToString("G");
                CmbCategories.SelectedValue = prod.CategoryId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validações mínimas na GUI (o backend também valida)
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Nome é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrice.Text, out var price) || price < 0)
            {
                MessageBox.Show("Preço inválido (deve ser >= 0).", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbCategories.SelectedValue == null)
            {
                MessageBox.Show("Selecione uma categoria.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var payload = new
            {
                name = TxtName.Text.Trim(),
                price = price,
                categoryId = (int)CmbCategories.SelectedValue
            };

            try
            {
                HttpResponseMessage resp;
                if (_productId.HasValue)
                {
                    resp = await _http.PutAsJsonAsync($"api/products/{_productId.Value}", payload);
                }
                else
                {
                    resp = await _http.PostAsJsonAsync("api/products", payload);
                }

                if (resp.IsSuccessStatusCode)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    // tenta ler mensagem de erro do corpo
                    var body = await resp.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro: {resp.StatusCode}\n{body}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
