using Core.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Client.Services;

namespace Web.Client.Components.Password
{
    public class PasswordListBase : ComponentBase
    {
        [Inject] public PasswordService PasswordService { get; set; }
        [Inject] public CategoryService CategoryService { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        

        // Liste complète des mots de passe récupérés depuis l'API
        protected List<PasswordEntry> AllPasswords { get; set; } = new();
        // Liste filtrée en fonction du terme de recherche
        protected List<PasswordEntry> FilteredPasswords { get; set; } = new();
        // Liste paginée affichée dans la table
        protected List<PasswordEntry> PagedPasswords { get; set; } = new();
        
        protected List<Category> Categories { get; set; } = new();
        private int? _selectedCategoryId = null;
        protected int? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                if (_selectedCategoryId != value)
                {
                    _selectedCategoryId = value;
                    CurrentPage = 1; // Réinitialiser la page
                    ApplyFilters();  // Recalculer les filtres
                }
            }
        }

        // Propriété de recherche avec setter qui déclenche le filtrage
        private string _searchTerm = "";
        protected string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    CurrentPage = 1; // Réinitialiser à la première page lors d'un changement de recherche
                    ApplyFilters();
                }
            }
        }

        // Pagination
        protected int CurrentPage { get; set; } = 1;
        
        private int _pageSize = 5;
        protected int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    CurrentPage = 1; // Réinitialiser à la première page lors d'un changement de pageSize
                    ApplyFilters();
                }
            }
        }
        
        protected int TotalPages { get; set; } = 1;

        // Charger les données après le premier rendu interactif (pour éviter les appels JS interop lors du prerendering)
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadPasswords();
                await LoadCategories();
                StateHasChanged();
            }
        }

        // Charge toutes les entrées depuis l'API
        protected async Task LoadPasswords()
        {
            AllPasswords = await PasswordService.GetPasswordsAsync();
            ApplyFilters();
        }
        
        // Charge les catégories depuis l'API
        protected async Task LoadCategories()
        {
            Categories = await CategoryService.GetCategoriesAsync();
            ApplyFilters();
        }

        // Applique le filtre de recherche et calcule la pagination
        protected void ApplyFilters()
        {
            // Filtrage en fonction du SearchTerm
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredPasswords = AllPasswords;
            }
            else
            {
                FilteredPasswords = AllPasswords.Where(p =>
                    p.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Username.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            
            
            // Filtrage par catégorie (si une catégorie est sélectionnée)
            if (SelectedCategoryId.HasValue)
            {
                FilteredPasswords = FilteredPasswords.Where(p => p.CategoryId == SelectedCategoryId.Value).ToList();
            }

            // Calcul du nombre total de pages
            TotalPages = (int)Math.Ceiling((double)FilteredPasswords.Count / PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            // Sélection des éléments pour la page courante
            PagedPasswords = FilteredPasswords
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        // Méthodes de pagination
        protected void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                ApplyFilters();
            }
        }

        protected void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                ApplyFilters();
            }
        }

        protected bool CanGoPrevious => CurrentPage > 1;
        protected bool CanGoNext => CurrentPage < TotalPages;

        // Ouvre le dialogue de détails (lecture seule)
        protected void ShowDetails(PasswordEntry entry)
        {
            var parameters = new DialogParameters { { "Password", entry } };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            DialogService.Show<PasswordDetailsDialog>("Détails du mot de passe", parameters, options);
        }

        // Ouvre le dialogue pour ajouter/modifier un mot de passe
        protected async Task OpenForm(PasswordEntry entry)
        {
            var parameters = new DialogParameters();
            if (entry != null)
            {
                // Crée une copie de l'entrée pour éviter la modification directe dans la liste
                parameters.Add("Password", new PasswordEntry
                {
                    Id = entry.Id,
                    Title = entry.Title,
                    Username = entry.Username,
                    Url = entry.Url,
                    EncryptedPassword = entry.EncryptedPassword,
                    CreatedAt = entry.CreatedAt,
                    UpdatedAt = entry.UpdatedAt,
                    Notes = entry.Notes,
                    CategoryId = entry.CategoryId,
                    Category = entry.Category,
                    UserId = entry.UserId,
                    User = entry.User
                });
            }
            else
            {
                parameters.Add("Password", new PasswordEntry
                {
                    CategoryId = 1,
                    Category = Categories.FirstOrDefault(c => c.Id == 1)
                });
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = DialogService.Show<PasswordFormDialog>("Ajouter / Modifier un mot de passe", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadPasswords();
                await LoadCategories();
                StateHasChanged();
            }
        }

        // Ouvre le dialogue de confirmation de suppression
        protected async Task OpenDelete(PasswordEntry entry)
        {
            var parameters = new DialogParameters { { "Password", entry } };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var dialog = DialogService.Show<PasswordDeleteDialog>("Confirmer la suppression", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadPasswords();
                await LoadCategories();
                StateHasChanged();
            }
        }
    }
}
