using Melior.InterviewQuestion.Data;
using Melior.InterviewQuestion.Services;
using Melior.InterviewQuestion.Types;
using Moq;

namespace InterviewQuestionTests
{
    public class PaymentServiceTests
    {
        private PaymentService _paymentService { get; set; }
        private readonly Mock<IAccountDataStore> _storeMock = new Mock<IAccountDataStore>();

        [SetUp]
        public void Setup()
        {
            //see comment in payment service: this would normally simply be: new PaymentService(_storeMock.Object);
            this._paymentService = new PaymentService();
            this._paymentService.store = _storeMock.Object;

        }

        [TestCase(PaymentScheme.Chaps)]
        [TestCase(PaymentScheme.FasterPayments)]
        [TestCase(PaymentScheme.Bacs)]
        public void MakePayment_PaymentSchemeNotInAllowedPaymentSchemes_ShouldReturnFalseAndNotRunUpdate(PaymentScheme paymentScheme)
        {
            var paymentRequest = new MakePaymentRequest()
            {
                DebtorAccountNumber = "123",
                PaymentDate = DateTime.Now,
                Amount = 1,
                CreditorAccountNumber = "456",
                PaymentScheme = paymentScheme
            };
            Account account = new Account()
            {
                AccountNumber = "123",
                AllowedPaymentSchemes = new List<PaymentScheme> { PaymentScheme.Chaps, PaymentScheme.Bacs, PaymentScheme.FasterPayments },
                Balance = 1,
                Status = AccountStatus.Live
            };
            account.AllowedPaymentSchemes.Remove(paymentScheme);
            this._storeMock.Setup(x=> x.GetAccount(paymentRequest.DebtorAccountNumber)).Returns(account);

            var response = this._paymentService.MakePayment(paymentRequest);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            this._storeMock.Verify(x=>x.UpdateAccount(account), Times.Never());
            
        }


        [TestCase(PaymentScheme.Bacs)]
        [TestCase(PaymentScheme.FasterPayments)]
        [TestCase(PaymentScheme.Chaps)]
        public void MakePayment_PaymentSchemeIsInAllowedPaymentSchemes_ShouldReturnTrueAndRunUpdate(PaymentScheme paymentScheme)
        {
            var paymentRequest = new MakePaymentRequest()
            {
                DebtorAccountNumber = "123",
                PaymentDate = DateTime.Now,
                Amount = 1,
                CreditorAccountNumber = "456",
                PaymentScheme = paymentScheme
            };
            Account account = new Account()
            {
                AccountNumber = "123",
                AllowedPaymentSchemes = new List<PaymentScheme>() { PaymentScheme.Chaps , PaymentScheme.Bacs, PaymentScheme.FasterPayments },
                Balance = 1,
                Status = AccountStatus.Live
            };
            this._storeMock.Setup(x => x.GetAccount(paymentRequest.DebtorAccountNumber)).Returns(account);

            var response = this._paymentService.MakePayment(paymentRequest);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            this._storeMock.Verify(x => x.UpdateAccount(account), Times.Once());
        }
        [Test]
        public void UpdateBalance_TakesAmountFromAccountAndUpdates()
        {
            var account = new Account()
            {
                Balance = 10,
            };
            var amount = 3;
            _paymentService.UpdateBalance(amount, account);

            Assert.IsTrue(account.Balance == 7);
            _storeMock.Verify(x=>x.UpdateAccount(account), Times.Once());
        }

    }
}