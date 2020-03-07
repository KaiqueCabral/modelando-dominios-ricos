using System.Collections.Generic;
using System.Linq;
using Flunt.Validations;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Entities;

namespace PaymentContext.Domain.Entities
{
    public class Student : Entity
    {
        private IList<Subscription> _subscriptions;
        public Student(Name name, Document document, Email email)
        {
            Name = name;
            Document = document;
            Email = email;
            _subscriptions = new List<Subscription>();

            AddNotifications(name, document, email);
        }

        public Name Name { get; set; }
        public Document Document { get; private set; }
        public Email Email { get; private set; }
        public Address Address { get; private set; }
        public IReadOnlyCollection<Subscription> Subscriptions { get { return _subscriptions.ToArray(); } }

        public void AddSubscription(Subscription subscription)
        {
            bool hasSubsciptionActive = false;
            foreach (var sub in Subscriptions)
            {
                if (sub.Active)
                {
                    hasSubsciptionActive = true;
                }
                //sub.Deactivate();
            }

            AddNotifications(new Contract()
                .Requires()
                .IsFalse(hasSubsciptionActive, "Student.Subscriptions", "Você já tem uma assinatura ativa.")
            );

            if (subscription.Payments.Count == 0)
            {
                AddNotification("Student.Subscriptions", "Está assinatura não possui pagamentos.");
            }

            if (hasSubsciptionActive)
            {
                AddNotification("Student.Subscriptions", "Você já tem uma assinatura ativa.");
            }

            if (Valid)
            {
                _subscriptions.Add(subscription);
            }
        }
    }
}