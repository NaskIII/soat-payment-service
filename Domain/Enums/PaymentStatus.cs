using System.ComponentModel;

namespace Domain.Enums
{
    public enum PaymentStatus
    {
        [Description("Pendente")]
        Pending = 0,

        [Description("Concluído")]
        Completed = 1,

        [Description("Falhou")]
        Failed = 2,

        [Description("Reembolsado")]
        Refunded = 3,

        [Description("Cancelado")]
        Cancelled = 4
    }
}
