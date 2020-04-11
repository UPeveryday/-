using System.Data.Entity.Migrations.Model;
using MaterialDesignThemes.Wpf;
using PortableEquipment.Pages.Book;
using PortableEquipment.Pages.Home;
using Stylet;
using StyletIoC;

namespace PortableEquipment.Pages
{
    public class ShellViewModel : Conductor<Screen>, IHandle<string>
    {
        [Inject]
        public IEventAggregator _eventAggregator;
        [Inject]
        public IContainer _container;
        [Inject]
        public IWindowManager _windowManager;

        public HomeViewModel homeViewModel;
        public HomeViewModel homeViewModel1;
        public BookViewModel bookvs;
        public BookViewModel bookvs2;
        protected override void OnViewLoaded()
        {

            var c = _container.GetAll(typeof(HomeViewModel));
            //homeViewModel = _container.Get<HomeViewModel>();
            //homeViewModel1 = _container.Get<HomeViewModel>();
            //bookvs = _container.Get<BookViewModel>();
            //bookvs2 = _container.Get<BookViewModel>();
            //homeViewModel1.DisplayName = "Home1";
            //bookvs2.DisplayName = "Book1";
            //ActivateItem(homeViewModel);
            //ActivateItem(bookvs);
            //ActivateItem(homeViewModel1);
            //ActivateItem(bookvs2);

            //DeactivateItem(homeViewModel);
            //var bookvs = _container.Get<BookViewModel>();
            //ActivateItem(bookvs);
        }

        public void changeitem()
        {
            ActiveItem = homeViewModel1;
        }
        protected override void OnInitialActivate()
        {
            //base.OnInitialActivate();
            //_eventAggregator.Subscribe(this);
        }

        public void Handle(string message)
        {
            homeViewModel.title = message;
            if (message == "ChangeItem")
            {
                ActiveItem = homeViewModel;
            }
        }
        public bool OpenOrclose { get; set; } = false;
        public void Click()
        {
            OpenOrclose = true;
        }
    }
}
