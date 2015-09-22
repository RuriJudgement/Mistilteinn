using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mistilteinn.ViewModels
{
    public class WelcomeWindowViewModel : ViewModelBase
    {
        public WelcomeWindowViewModel(EventWaitHandle waitHandle)
        {
            WaitHandle = waitHandle;
        }

        public EventWaitHandle WaitHandle { get; private set; }
        public Project Project { get; set; }
    }
}
