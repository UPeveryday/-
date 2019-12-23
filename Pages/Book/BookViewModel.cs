using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;
using StyletIoC;

namespace PortableEquipment.Pages.Book
{
    public class BookViewModel:Screen,IHandle<string>
    {
        [Inject]
        public IEventAggregator _eventAggregator;
        public BookViewModel()
        {
            DisplayName = "Book";
        }

        public string text { get; set; } = "My text";
        public void Stop()
        {
            Pisher();
        }
        public void Pisher() => _eventAggregator.Publish("hello my name ");

        public void Handle(string message)
        {
           // throw new NotImplementedException();
        }
    }
}
