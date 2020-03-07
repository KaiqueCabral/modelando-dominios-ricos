using Flunt.Validations;
using PaymentContext.Domain.Enums;
using PaymentContext.Shared.ValueObjects;

namespace PaymentContext.Domain.ValueObjects
{
    public class Document : ValueObject
    {
        public string Number { get; private set; }
        public EDocumentType Type { get; private set; }

        public Document(string number, EDocumentType type)
        {
            Number = number;
            Type = type;

            AddNotifications(new Contract()
                .Requires()
                .IsTrue(Validate(), "Document.Number", "Documento inválido.")
            );
        }

        public bool Validate()
        {
            switch (Type)
            {
                case EDocumentType.CNPJ:
                    return (Number.Length == 14);
                case EDocumentType.CPF:
                    return (Number.Length == 11);
                default:
                    return false;
            }
        }
    }
}