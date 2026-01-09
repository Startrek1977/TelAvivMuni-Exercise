using System.Collections;

namespace TelAvivMuni_Exercise.Services
{
    public interface IDialogService
    {
        object? ShowBrowseDialog(IEnumerable items, string title, object? currentSelection);
    }
}
