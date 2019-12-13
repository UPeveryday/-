using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers
{
    public class LoginRespontory<T> : ILogin<T>
    {
        public T Login()
        {
            if (typeof(T).FullName=="string")
            {
                return default;
            }
            else
            {
                return default;
            }
        }
    }
}
