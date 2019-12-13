using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Windows;
using StyletIoC;

namespace PortableEquipment.ViewModels
{
    public class LoginViewModel : Screen
    {
        [Inject]//特性注入
        private IEventAggregator _eventAggregator;
        [Inject]//特性注入
        private Servers.IEntityServer _jsEntities;
        [Inject]
        private IWindowManager _windowManger;
        [Inject]
        private SignupViewModel _SignupViewModel;
        [Inject]
        private MainViewModel _MainViewModel;
        public string LoginName { get; set; }
        public string PassWord { get; set; }
        public string HidePassword { get; set; }
        public string Usernamehnit { get; set; }
        public Visibility WindowVisibility { get; set; } = Visibility.Visible;
        public ObservableCollection<string> UserList { get; set; }
        [Inject]
        private IEnumerable<Servers.ILogin<string>> _login;
        public LoginViewModel()
        {
            //Servers.IEntityServer jsEntities
            //  _eventAggregator = eventAggregator;//构造函数注入
        }
        public void WindowLoad()
        {
            _jsEntities.entitiesmodel.usertables.Load();
            UserList = new ObservableCollection<string>();
            foreach (var item in _jsEntities.entitiesmodel.usertables.Select(a => a.username).ToList())
            {
                UserList.Add(item);
            }
            if (UserList.Count >= 1)
                LoginName = UserList.ToArray()[0];
        }
        public void ShowSignupViewModel()
        {
            //_SignupViewModel = new SignupViewModel(_windowManger);
            _windowManger.ShowWindow(_SignupViewModel);

        }
        public void ShowMainViewModel()
        {
            _windowManger.ShowDialog(_MainViewModel);
        }
        public void Login()
        {
            //  _login.ToArray()[0].Login();
            var un = _jsEntities.entitiesmodel.usertables.Where(p => p.username == LoginName).ToList();
            if (un.Count >= 1)
            {
                Usernamehnit = "UserName";
                if (un[0].password == PassWord)
                {
                    Pisher();
                    ShowMainViewModel();
                    Close();
                }
                else
                {
                    PassWord = "密码错误";
                }
            }
            else
            {
                LoginName = string.Empty;
                PassWord = string.Empty;
                Usernamehnit = "未找到此用户";
            }
        }
        public void NameChanged()
        {
            Usernamehnit = "UserName";
        }
        public MessageBoxResult ShowMessage() => _windowManger.ShowMessageBox("Hello", "警告", MessageBoxButton.YesNo, MessageBoxImage.Information);
        public void Close() => this.RequestClose();
        public void Pisher() => _eventAggregator.Publish("手动调压");

    }
}
