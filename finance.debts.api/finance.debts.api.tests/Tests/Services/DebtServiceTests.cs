using Moq;
using FluentAssertions;
using finance.debts.api.Services;
using finance.debts.domain.Enums;
using finance.debts.domain.Entities;
using finance.debts.domain.Interfaces;

namespace finance.debts.api.tests.Tests.Services
{
    public class DebtServiceTests
    {
        private readonly Mock<IDebtRepository> _debtRepositoryMock;
        private readonly Mock<IProcessingLogRepository> _logRepositoryMock;
        private readonly DebtService _service;

        public DebtServiceTests()
        {
            _debtRepositoryMock = new Mock<IDebtRepository>();
            _logRepositoryMock = new Mock<IProcessingLogRepository>();

            _service = new DebtService(
                _debtRepositoryMock.Object,
                _logRepositoryMock.Object
            );
        }

        [Fact]
        public async Task ProcessDebt_Should_Process_When_StatusIsPending()
        {
            // Arrange
            var debt = new Debt(
                debtId: 1,
                clientId: 1,
                amountDue: 100,
                correlationId: null
            );

            _debtRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(debt);

            _debtRepositoryMock
                .Setup(x => x.TryProcessAsync(It.IsAny<Debt>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.ProcessDebtAsync(1, Guid.NewGuid());

            // Assert
            result.Should().Be("Debt 1 processed successfully");

            _debtRepositoryMock.Verify(x =>
                x.TryProcessAsync(It.Is<Debt>(d =>
                    d.StatusId == ProcessingStatus.Processed &&
                    d.AmountPaid == 100
                )),
                Times.Once);

            _logRepositoryMock.Verify(x =>
                x.AddAsync(It.Is<ProcessingLog>(log =>
                    log.DebtId == 1 &&
                    log.StatusId == (int)ProcessingStatus.Processed &&
                    log.Message.Contains("sucesso")
                )),
                Times.Once);
        }
        [Fact]
        public async Task ProcessDebt_Should_ThrowException_When_DebtAlreadyProcessed()
        {
            // Arrange
            var debt = new Debt(1, 1, 100, null);
            debt.Process(Guid.NewGuid());

            _debtRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(debt);

            // Act
            var act = async () => await _service.ProcessDebtAsync(1, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Dívida já processada");

            // valida que log foi salvo
            _logRepositoryMock.Verify(x =>
                x.AddAsync(It.Is<ProcessingLog>(log =>
                    log.DebtId == 1 &&
                    log.StatusId == (int)ProcessingStatus.Error &&
                    log.Message == "Dívida já processada"
                )),
                Times.Once
            );

            _debtRepositoryMock.Verify(x =>
                x.TryProcessAsync(It.IsAny<Debt>()),
                Times.Never
            );

            // não deve atualizar
            _debtRepositoryMock.Verify(x =>
                x.TryProcessAsync(It.IsAny<Debt>()),
                Times.Never);
        }
        [Fact]
        public async Task ProcessDebt_Should_ThrowException_When_DebtNotFound()
        {
            // Arrange
            _debtRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Debt?)null);

            // Act
            var act = async () => await _service.ProcessDebtAsync(1, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Dívida não encontrada");

            _logRepositoryMock.Verify(x =>
                x.AddAsync(It.Is<ProcessingLog>(log =>
                    log.DebtId == 1 &&
                    log.StatusId == -1 &&
                    log.Message.Contains("Dívida não encontrada")
                )),
                Times.Once
            );
        }
        [Fact]
        public async Task ProcessDebt_Should_ThrowException_When_IdIsInvalid()
        {
            // Act
            var act = async () => await _service.ProcessDebtAsync(0, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("DebtId inválido");

            _logRepositoryMock.Verify(x =>
                x.AddAsync(It.IsAny<ProcessingLog>()),
                Times.Once
            );

            _debtRepositoryMock.Verify(x =>
            x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);
        }

    }
}