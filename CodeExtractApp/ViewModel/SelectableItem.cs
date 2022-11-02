using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExtractApp.ViewModel
{
    public class SelectableItem<T>
    {
        public T Value { get; set; }
        public bool IsSelected { get; set; }
    }
}
