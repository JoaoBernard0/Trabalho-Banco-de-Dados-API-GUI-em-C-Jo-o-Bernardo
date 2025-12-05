using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace InventoryGui
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7245/") }; // ajuste porta se necessário

        public MainWindow()
        {
            InitializeComponent();
            _ = LoadProductsAsync();
        }

        private async Task LoadProductsAsync(string? name = null)
        {
            try
            {
                var url = "api/products";
                if (!string.IsNullOrWhiteSpace(name)) url += $"?name={Uri.EscapeDataString(name)}";
                var list = await _http.GetFromJsonAsync<List<ProductDto>>(url);
                ProductsGrid.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produtos: {ex.Message}");
            }
        }

        private async void OnSearch(object sender, RoutedEventArgs e) => await LoadProductsAsync(SearchBox.Text);

        private async void OnNewProduct(object sender, RoutedEventArgs e)
        {
            var win = new ProductEditWindow(_http, null) { Owner = this };
            var res = win.ShowDialog();
            if (res == true) await LoadProductsAsync();
        }

        private async void OnEdit(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is ProductDto p)
            {
                var win = new ProductEditWindow(_http, p.Id) { Owner = this };
                var res = win.ShowDialog();
                if (res == true) await LoadProductsAsync();
            }
        }

        private async void OnDelete(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is ProductDto p)
            {
                var ask = MessageBox.Show($"Deletar {p.Name}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ask != MessageBoxResult.Yes) return;

                var resp = await _http.DeleteAsync($"api/products/{p.Id}");
                if (resp.IsSuccessStatusCode) await LoadProductsAsync();
                else
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro: {resp.StatusCode}\n{body}");
                }
            }
        }
    }
}
