namespace BookManagement.Services.Dialog
{
    public interface IDialogService
    {
        bool ShowConfirmation(string title, string message);
    }
}
