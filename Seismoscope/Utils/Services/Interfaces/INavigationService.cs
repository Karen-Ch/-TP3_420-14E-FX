using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services.Interfaces
{
    public interface INavigationService
    {
        BaseViewModel CurrentView { get; }
        void NavigateTo<T>() where T : BaseViewModel;
        void NavigateTo<T>(object parameter) where T : BaseViewModel;
        
    }
}
