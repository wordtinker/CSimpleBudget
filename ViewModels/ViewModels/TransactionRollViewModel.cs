using Models;

namespace ViewModels
{
    public class TransactionRollViewModel
    {
        private Account account;
        private IUITransactionRollService service;

        public TransactionRollViewModel(Item item, IUITransactionRollService service)
        {
            this.account = item.account;
            this.service = service;
        }
    }
}
