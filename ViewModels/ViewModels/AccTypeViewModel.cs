using Models;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{

    public class AccTypeItem
    {
        public string Name { get; }

        public AccTypeItem(string name)
        {
            Name = name;
        }
    }

    public class AccTypeViewModel : BindableBase
    {
        // TODO
        public IEnumerable<AccTypeItem> AccTypes
        {
            get
            {
                return from t in Core.Instance.AccountTypes
                       select new AccTypeItem(t);
            }
        }
    }
}
