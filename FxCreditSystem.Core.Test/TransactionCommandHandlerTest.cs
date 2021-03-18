using System.Threading.Tasks;
using DeepEqual.Syntax;
using FxCreditSystem.Common;
using Moq;
using Xunit;

namespace FxCreditSystem.Core.Test
{
    public class TransactionCommandHandlerTest
    {

        [Fact]
        public async Task Add_ShouldSucceed()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker();
            var addTransactionCommand = addTransactionCommandFaker.Generate();

            var mockTransactionRepository = new Mock<ITransactionRepository>();

            TransactionCommandHandler transactionCommandHandler = new TransactionCommandHandler(mockTransactionRepository.Object);
            await transactionCommandHandler.HandleAsync(addTransactionCommand);

            mockTransactionRepository.Verify(tr => tr.Add(It.Is<Common.Entities.AddTransactionCommand>(ta => ta.IsDeepEqual(addTransactionCommand))));
            mockTransactionRepository.VerifyNoOtherCalls();
        }
    }
}
