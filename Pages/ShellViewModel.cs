using System.Data.Entity.Migrations.Model;
using PortableEquipment.Pages.Book;
using PortableEquipment.Pages.Home;
using Stylet;
using StyletIoC;

namespace PortableEquipment.Pages
{
    public class ShellViewModel : Conductor<Screen>.Collection.AllActive, IHandle<string>
    {
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
            homeViewModel = _container.Get<HomeViewModel>();
            homeViewModel1 = _container.Get<HomeViewModel>();
            bookvs = _container.Get<BookViewModel>();
            bookvs2 = _container.Get<BookViewModel>();
            homeViewModel1.DisplayName = "Home1";
            bookvs2.DisplayName = "Book1";
            ActivateItem(homeViewModel);
            ActivateItem(bookvs);
            ActivateItem(homeViewModel1);
            ActivateItem(bookvs2);
           
            
            //DeactivateItem(homeViewModel);
            //var bookvs = _container.Get<BookViewModel>();
            //ActivateItem(bookvs);
        }
        protected override void OnInitialActivate()
        {
        }

        public void Handle(string message)
        {
            homeViewModel.title = message;
        }
    }
}
