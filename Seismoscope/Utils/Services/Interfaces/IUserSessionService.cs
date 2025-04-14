using System.ComponentModel;

namespace Seismoscope.Utils.Services.Interfaces
{
    public interface IUserSessionService : INotifyPropertyChanged
    {
        User ConnectedUser { get; set; }

        bool IsUserConnected { get; }
    }
}
