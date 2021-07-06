using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.ToolKits.MVVM
{
    public interface IMessageDialogService
    {
        Task<bool> ShowAsync(string message,string title=null);

        bool Show(string message, string title = null);
         
        Task<bool> ConfirmAsync(string message, string title = null);

        bool Confirm (string message, string title = null);

        Task<bool> PopupAsync(object uIElement, string title = null);
         
        bool Popup (object uIElement, string title = null);
    }
}
