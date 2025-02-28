using Core.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Client.Services;

namespace Web.Client.Components.Password
{
    public class PasswordFormDialogBase : ComponentBase
    {
        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
        [Inject] public PasswordService PasswordService { get; set; }
        [Inject] public CategoryService CategoryService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }


        [Parameter] public PasswordEntry Password { get; set; }

        // Liste des catégories pour le sélecteur
        protected List<Category> Categories { get; set; } = new();

        // Pour gérer l'affichage/masquage du mot de passe
        protected InputType _passwordInputType = InputType.Password;

        // Pour le popover du générateur
        protected bool _showGeneratorPopover = false;
        protected int _selectedLength = 16; // Par défaut : 16 caractères
        public bool _includeLowercase { get; set; } = true;
        public bool _includeUppercase { get; set; } = true;
        public bool _includeDigits { get; set; } = true;
        public bool _includeSymbols { get; set; } = true;



        protected override async Task OnInitializedAsync()
        {
            Categories = await CategoryService.GetCategoriesAsync();
            
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            bool isNew = Password.Id == 0;

            try
            {
                if (isNew)
                {
                    await PasswordService.CreatePasswordAsync(Password);
                    Snackbar.Add("Mot de passe ajouté avec succès !", Severity.Success);
                }
                else
                {
                    await PasswordService.UpdatePasswordAsync(Password);
                    Snackbar.Add("Mot de passe mis à jour avec succès !", Severity.Success);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Une erreur est survenue : {ex.Message}", Severity.Error);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }

        protected void Close() => MudDialog.Cancel();

        // Bascule l'affichage du mot de passe
        protected void TogglePasswordVisibility()
        {
            _passwordInputType = _passwordInputType == InputType.Password ? InputType.Text : InputType.Password;
        }

        // Ouvre/ferme le popover du générateur
        protected void ToggleGeneratorPopover()
        {
            _showGeneratorPopover = !_showGeneratorPopover;
        }

        protected void CloseGeneratorPopover()
        {
            _showGeneratorPopover = false;
        }

        // Appelé lorsque l'utilisateur clique sur "Générer"
        protected void OnGenerateClicked()
        {
            Password.DecryptedPassword = GenerateCustomPassword(_selectedLength, _includeLowercase, _includeUppercase, _includeDigits, _includeSymbols);
            _showGeneratorPopover = false;
            StateHasChanged();
        }

        // Génère un mot de passe personnalisé en fonction des critères sélectionnés
        private string GenerateCustomPassword(int length, bool includeLower, bool includeUpper, bool includeDigits, bool includeSymbols)
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digitChars = "0123456789";
            const string symbolChars = "!@#$%^&*()";
            string availableChars = "";
            if (includeLower) availableChars += lowerChars;
            if (includeUpper) availableChars += upperChars;
            if (includeDigits) availableChars += digitChars;
            if (includeSymbols) availableChars += symbolChars;
            if (string.IsNullOrEmpty(availableChars))
                return "";

            Random rnd = new Random();
            char[] pwd = new char[length];
            int index = 0;

            // Forcer la présence d'au moins un caractère de chaque type sélectionné (si possible)
            if (includeLower && index < length)
                pwd[index++] = lowerChars[rnd.Next(lowerChars.Length)];
            if (includeUpper && index < length)
                pwd[index++] = upperChars[rnd.Next(upperChars.Length)];
            if (includeDigits && index < length)
                pwd[index++] = digitChars[rnd.Next(digitChars.Length)];
            if (includeSymbols && index < length)
                pwd[index++] = symbolChars[rnd.Next(symbolChars.Length)];

            for (; index < length; index++)
            {
                pwd[index] = availableChars[rnd.Next(availableChars.Length)];
            }

            // Mélanger les caractères pour éviter que les caractères forcés soient toujours en début de chaîne
            return new string(pwd.OrderBy(x => rnd.Next()).ToArray());
        }
    }
}
