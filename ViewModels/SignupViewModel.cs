using Model;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PortableEquipment.ViewModels
{
    public class SignupViewModel : Screen, IDataErrorInfo
    {
        private jsEntities _jsEntities;
        public IWindowManager  _windowManager { get; set; }
        public LoginViewModel _LoginViewModel { get; private set; }
        public bool WindowIsEable { get; set; } = true;
        public string Username { get; set; }
        public string Password { get; set; }
        public string MarkId { get; set; }
        [StringLength(11, MinimumLength = 8)]
        public string PhoneNum { get; set; }
        public string Emial { get; set; }
        public string other { get; set; }
        public string ConfirePassWord { get; set; }
        public string ZcMessage { get; set; } = "立即注册";
        public string hnit { get; set; } = "用户名";
        public int NeedDo { get; set; } = 0;
        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                var vc = new ValidationContext(this, null, null);
                vc.MemberName = columnName;
                var res = new List<ValidationResult>();
                var result = System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(this.GetType().GetProperty(columnName).GetValue(this, null), vc, res);
                if (res.Count > 0)
                {
                    return string.Join(Environment.NewLine, res.Select(r => r.ErrorMessage).ToArray());
                }
                return string.Empty;
            }
        }
        public SignupViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            //_LoginViewModel = new LoginViewModel(_windowManager, "hello");
            _jsEntities = new jsEntities();
            _jsEntities.usertables.Load();
        }
        public void Adduser()
        {
            if (Password == ConfirePassWord)
            {

                IQueryable<usertable> data = from v in _jsEntities.usertables
                                             where v.username == Username
                                             select v;
                var y = data.ToList();
                usertable Newuser = new usertable
                {
                    username = Username,
                    password = Password,
                    markid = MarkId,
                    phonenum = PhoneNum,
                    email = Emial,
                    other = other
                };
                if (data.ToList().Count <= 0)
                {
                    _jsEntities.usertables.Add(Newuser);
                    _jsEntities.SaveChanges();
                    MulTime(3);
                }
                else
                {
                    if (NeedDo == 1)
                    {
                        Newuser.id = y[0].id;
                        foreach (var c in data)
                        {
                            c.id = Newuser.id;
                            c.markid = Newuser.markid;
                            c.other = Newuser.other;
                            c.password = Newuser.password;
                            c.phonenum = Newuser.phonenum;
                            c.username = Newuser.username;
                        }
                        _jsEntities.SaveChanges();
                        MulTime(3);
                    }
                    else
                    {
                        Username = string.Empty;
                        hnit = "用户已存在";
                    }
                }
            }
            else
            {
                Password = "";
                ConfirePassWord = "两次密码不一致";
            }

        }
        private void MulTime(int p)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                for (int i = 0; i < p; i++)
                {
                    ZcMessage = "注册成功" + "(" + (p - i).ToString() + ")";
                    await Task.Delay(1000);
                }
                Close();
            });
        }
        public void InitData()
        {
            _jsEntities = new jsEntities();
            _jsEntities.usertables.Load();
            Username = "";
            Password = "";
            MarkId = "";
            Emial = "";
            PhoneNum = "";
            other = "";
            ConfirePassWord = "";
        }
        public void Close() => this.RequestClose();

    }
}
