using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melior.InterviewQuestion.Types;

namespace Melior.InterviewQuestion.Data
{
    public interface IAccountDataStore
    {
        public Account GetAccount(string accountNumber);
        public void UpdateAccount(Account account);
    }
}
