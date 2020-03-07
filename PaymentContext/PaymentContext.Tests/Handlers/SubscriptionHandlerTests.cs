using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Handlers;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Tests.Mocks;
// using System;
// using PaymentContext.Domain.Entities;
// using PaymentContext.Domain.ValueObjects;

namespace PaymentContext.Tests
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ShouldReturnErrorWhenDocumentExists()
        {
            var handler = new SubscriptionHandler(new FakeStudentRepository(), new FakeEmailService());
            var command = new CreateBoletoSubscriptionCommand();
            command.FirstName = "Bruce";
            command.LastName = "Wayne";
            command.Document = "99999999999";
            command.Email = "kaique.11@hotmail.com";
            command.BarCode = "123456789";
            command.BoletoNumber = "12345678912";
            command.PaymentNumber = "123123";
            command.PaidDate = DateTime.Now;
            command.ExpireDate = DateTime.Now.AddMonths(1);
            command.Total = 500;
            command.TotalPaid = 500;
            command.Payer = "Wayne Corp";
            command.PayerDocument = "12312312312";
            command.PayerDocumentType = EDocumentType.CPF;
            command.PayerEmail = "batman@dc.com";
            command.Street = "Rua 1";
            command.Number = "123";
            command.Neighborhood = "Bairro";
            command.City = "SJRP";
            command.State = "SP";
            command.Country = "Brasil";
            command.ZipCode = "12999999";

            handler.Handle(command);
            Assert.AreEqual(false, handler.Valid);
        }
    }
}
