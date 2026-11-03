using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using systemapps.Repositories;

namespace systemapps.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MainViewModel : ViewModelBase
    {
        private readonly IDatabaseRepository _dbRepository;
        private readonly ISecurityService _securityService;

        public MainViewModel(IDatabaseRepository dbRepository, ISecurityService securityService)
        {
            _dbRepository = dbRepository;
            _securityService = securityService;
        }

        // Connection Properties
        private string _dbIp;
        public string DbIp 
        { 
            get => _dbIp; 
            set { _dbIp = value; OnPropertyChanged(); } 
        }

        private string _dbName;
        public string DbName 
        { 
            get => _dbName; 
            set { _dbName = value; OnPropertyChanged(); } 
        }

        private string _dbUser;
        public string DbUser 
        { 
            get => _dbUser; 
            set { _dbUser = value; OnPropertyChanged(); } 
        }

        private string _dbPassword;
        public string DbPassword 
        { 
            get => _dbPassword; 
            set { _dbPassword = value; OnPropertyChanged(); } 
        }

        private bool _isConnected;
        public bool IsConnected 
        { 
            get => _isConnected; 
            set { _isConnected = value; OnPropertyChanged(); } 
        }

        private string _connectionStatusText = "Disconnected";
        public string ConnectionStatusText 
        { 
            get => _connectionStatusText; 
            set { _connectionStatusText = value; OnPropertyChanged(); } 
        }

        // Commands
        public ICommand TestConnectionCommand { get; }

        public MainViewModel(IDatabaseRepository dbRepository, ISecurityService securityService, ICommand testCommand) 
            : this(dbRepository, securityService)
        {
            TestConnectionCommand = testCommand;
        }

        public async Task TestConnectionAsync()
        {
            IsConnected = false;
            ConnectionStatusText = "Testing...";
            
            bool result = await _dbRepository.TestConnectionAsync(DbIp, DbName, DbUser, DbPassword);
            
            IsConnected = result;
            ConnectionStatusText = result ? "Data Connected" : "Connection Fail!";
        }
    }
}
