using System;
using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : Notifiable, IHandler<CreateBoletoSubscriptionCommand>, IHandler<CreatePayPalSubscriptionCommand>
    {
        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;
        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            //Fail Fast Validations
            command.Validate();

            if (command.Invalid)
            {
                return new CommandResult(false, "Não foi possível realizar o seu cadastro.");
            }

            // Verify if Document already exists
            if (_repository.DocumentExists(command.Document))
            {
                AddNotification("Document", "Este CPF já está em uso.");
            }

            // Verify if Email already exists
            if (_repository.EmailExists(command.Email))
            {
                AddNotification("Email", "Este Email já está em uso.");
            }

            // Generate the Value Objects            
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(
                command.BarCode,
                command.BoletoNumber,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                document,
                address,
                email);

            //Relationship
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            //Group of Validations
            AddNotifications(name, document, email, address, student, subscription, payment);

            //Check Notifications
            if (Invalid)
                return new CommandResult(false, "Não foi possível realizar a sua assinatura.");

            //Save the info
            _repository.CreateSubscription(student);

            //Send a "Welcome" e-mail
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Batman Test", "Sua assinatura foi criada.");

            return new CommandResult(true, "Assinatura realizada com sucesso.");
        }
        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            //Fail Fast Validations
            //command.Validate();

            // if (command.Invalid)
            // {
            //     return new CommandResult(false, "Não foi possível realizar o seu cadastro.");
            // }

            // Verify if Document already exists
            if (_repository.DocumentExists(command.Document))
            {
                AddNotification("Document", "Este CPF já está em uso.");
            }

            // Verify if Email already exists
            if (_repository.EmailExists(command.Email))
            {
                AddNotification("Email", "Este Email já está em uso.");
            }

            // Generate the Value Objects            
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(
                command.TransactionCode,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                document,
                address,
                email);

            //Relationship
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            //Group of Validations
            AddNotifications(name, document, email, address, student, subscription, payment);

            //Save the info
            _repository.CreateSubscription(student);

            //Send a "Welcome" e-mail
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Batman Test", "Sua assinatura foi criada.");

            return new CommandResult(true, "Assinatura realizada com sucesso.");
        }
    }
}