namespace Nop.Plugin.Payments.TodoPago
{
    public enum EstadosTransaccion
    {
        Pendiente = 1,
        Procesando = 2,
        Espera = 3,
        Completado = 4,
        Cancelado = 5,
        Reembolsado = 6,
        Fallido = 7
    }
}
