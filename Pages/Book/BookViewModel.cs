using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;
using StyletIoC;

namespace PortableEquipment.Pages.Book
{
    public class BookViewModel : Screen, IHandle<string>
    {
        [Inject]
        public IEventAggregator _eventAggregator;
        public BookViewModel()
        {
            DisplayName = "Book";
        }
        protected override void OnInitialActivate()
        {
            //base.OnInitialActivate();
            //_eventAggregator.Subscribe(this);
        }
        public string text { get; set; } = "My text";
        public void Stop()
        {
            Pisher();
        }

        public void Pisher() => _eventAggregator.Publish("ChangeItem");

        public void Handle(string message)
        {
            // throw new NotImplementedException();
        }
    }
}
