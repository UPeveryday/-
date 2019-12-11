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

namespace PortableEquipment.ViewModels
{
    public class LoginViewModel : Screen
    {
        private jsEntities _jsEntities;
        private IWindowManager _windowManger;
        private SignupViewModel _SignupViewModel;
        private MainViewModel _MainViewModel;
        public string LoginName { get; set; } 
        public string PassWord { get; set; }
        public string HidePassword { get; set; }
        public string Usernamehnit { get; set; }

        public Visibility WindowVisibility { get; set; } = Visibility.Visible;

        public ObservableCollection<string> UserList { get; set; }
        public LoginViewModel(IWindowManager windowManager, SignupViewModel signupViewModel, MainViewModel mainViewModel)
        {
            _windowManger = windowManager;
            _SignupViewModel = signupViewModel;
            _MainViewModel = mainViewModel;
            _jsEntities = new jsEntities();
            _jsEntities.usertables.Load();
            UserList = new ObservableCollection<string>();
            foreach (var item in _jsEntities.usertables.Select(a => a.username).ToList())
            {
                UserList.Add(item);
            }
            if (UserList.Count >= 1)
                LoginName = UserList.ToArray()[0];
        }
        public LoginViewModel(IWindowManager windowManager,string name)
        {
            _windowManger = windowManager;
            LoginName = name;
        }
        public void ShowSignupViewModel()
        {
            _SignupViewModel = new SignupViewModel(_windowManger);
            _windowManger.ShowDialog(_SignupViewModel);

        }
        public void ShowMainViewModel()
        {
            _windowManger.ShowDialog(_MainViewModel);
        }
        public void Login()
        {
            var un = _jsEntities.usertables.Where(p => p.username == LoginName).ToList();
            if (un.Count >= 1)
            {
                Usernamehnit = "UserName";
                if (un[0].password == PassWord)
                {
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

        public MessageBoxResult ShowMessage() => _windowManger.ShowMessageBox("Hello","警告",MessageBoxButton.YesNo,MessageBoxImage.Information);

        public void Close() => this.RequestClose();

        
    }
}
