using Nop.Core;
using Nop.Plugin.Payments.TodoPago.Domain;

namespace Nop.Plugin.Payments.TodoPago.Services
{
    public partial interface ITodoPagoTransactionService
    {
        void deleteTodoPagoTransactionRecord(TodoPagoTransactionRecord todoPagoTransactionRecord);

        IPagedList<TodoPagoTransactionRecord> findAll(int pageIndex = 0 , int pageSize = int.MaxValue);

        TodoPagoTransactionRecord findById(int todoPagoTransactionRecordId);

        TodoPagoTransactionRecord findByOrdenId(int Id);

        void insertTodoPagoTransactionRecord(TodoPagoTransactionRecord todoPagoTransactionRecord);

        void updateTodoPagoTransactionRecord(TodoPagoTransactionRecord todoPagoTransactionRecord);
    }
}
