using System;
using System.Collections.Generic;
using System.Text;

namespace GetLocDevice
{
    public interface INotification
    {
        void CreateNotification(String title, String message);
    }
}
