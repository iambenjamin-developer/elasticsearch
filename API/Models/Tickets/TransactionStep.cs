using System.ComponentModel;

namespace API.Models.Tickets
{
    public enum TransactionStep
    {

        [Description("INICIO")] 
        START = 0,

        [Description("INVENTARIO")]
        INVENTORY = 1,

        [Description("SOLICITUD DE RESERVA")]
        BOOKED_REQUEST = 5,

        [Description("ESPERA DE INSTALACIÓN")]
        WAITING_SETUP = 7,
       
        [Description("CANCELAR")]
        CANCEL = 11,

    }
}
