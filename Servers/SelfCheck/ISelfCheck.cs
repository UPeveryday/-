using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.SelfCheck
{
    public interface ISelfCheck
    {
        Task<SelfCheckMesssage> ComPortCheck();
        Task<bool> SeleCheck(CancellationToken token);
    }
}
