using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services
{
    public class UserSessionService : IUserSessionService
    {
        private User? _connectedUser;

        public User ConnectedUser
        {
            get => _connectedUser!;
            set
            {
                _connectedUser = value;
                OnPropertyChanged(nameof(ConnectedUser));
                OnPropertyChanged(nameof(IsUserConnected));
            }
        }

        public bool IsUserConnected => _connectedUser != null;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
