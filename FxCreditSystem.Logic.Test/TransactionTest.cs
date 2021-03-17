using System.Threading.Tasks;
using DeepEqual.Syntax;
using FxCreditSystem.Common;
using Moq;
using Xunit;

namespace FxCreditSystem.Logic.Test
{
    public class TransactionTest
    {

        [Fact]
        public async Task Add_ShouldSucceed()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker();
            var transactionAdd = transactionAddFaker.Generate();

            var mockTransactionRepository = new Mock<ITransactionRepository>();

            Transaction transaction = new Transaction(mockTransactionRepository.Object);
            await transaction.Add(transactionAdd);

            mockTransactionRepository.Verify(tr => tr.Add(It.Is<Common.Entities.TransactionAdd>(ta => ta.IsDeepEqual(transactionAdd))));
            mockTransactionRepository.VerifyNoOtherCalls();
        }
    }
}
