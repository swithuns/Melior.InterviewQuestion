using Melior.InterviewQuestion.Data;
using Melior.InterviewQuestion.Types;
using System.Configuration;
using System.Security.Principal;

namespace Melior.InterviewQuestion.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly string dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
        //public as a workaround for testing, ideally this store would be passed via constructor (dependency injection) but I didn't want to alter the
        //constructors props in case you were running it within a wider solution of your own, a better setup would be:
        //private readonly IAccountDataStore _store;
        //public PaymentService(IAccountDataStore store)
        //{
              //this._store = store; 
        //}
        // with backup handling once at a higher level
        public IAccountDataStore store;
        public PaymentService()
        {
            store = dataStoreType == "Backup" ? new BackupAccountDataStore() : new AccountDataStore();
        }


        public void UpdateBalance(decimal amount, Account account)
        {
            if (account == null) return;
            account.Balance -= amount;
            store.UpdateAccount(account);
        }



        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            Account account = store.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult();

            result.Success = account != null && (account.AllowedPaymentSchemes.Contains(request.PaymentScheme));

            if (result.Success) UpdateBalance(request.Amount, account);

            return result;
        }
    }
}
