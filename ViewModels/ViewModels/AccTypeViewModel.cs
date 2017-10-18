using Models;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{

    /// <summary>
    /// Simple container for storing Account type object(string).
    /// </summary>
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
        public IEnumerable<AccTypeItem> AccTypes
        {
            get
            {
                return from t in Core.Instance.AccountTypes
                       select new AccTypeItem(t);
            }
        }

        public bool DeleteAccType(AccTypeItem item)
        {
            return Core.Instance.DeleteAccType(item.Name);
        }

        public bool AddAccType(string accTypeName)
        {
            return Core.Instance.AddAccType(accTypeName);
        }

        public AccTypeViewModel()
        {
            Core.Instance.AccountTypes.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged(nameof(AccTypes));
            };
        }
    }
}
