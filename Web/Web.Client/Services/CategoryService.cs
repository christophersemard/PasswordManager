using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Models;

namespace Web.Client.Services
{
    public class CategoryService 
    {
        private readonly HttpClient _httpClient;
        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<Category>>("api/categories");
            Console.WriteLine("GET CATEGORIES");
            Console.WriteLine(result);
            return result ?? new List<Category>();
        }
        
        
    }
}