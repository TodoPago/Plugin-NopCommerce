using Nop.Core;
using Nop.Plugin.Payments.TodoPago.Domain;

namespace Nop.Plugin.Payments.TodoPago.Services
{
    public partial interface ITodoPagoAddressBookService
    {
        void deleteTodoPagoAddressBookRecord(TodoPagoAddressBookRecord todoPagoAddressBookRecord);

        IPagedList<TodoPagoAddressBookRecord> findAll(int pageIndex = 0, int pageSize = int.MaxValue);

        TodoPagoAddressBookRecord findById(int todoPagoTransactionRecordId);

        TodoPagoAddressBookRecord findByHash(string hash);

        void insertTodoPagoAddressBookRecord(TodoPagoAddressBookRecord todoPagoAddressBookRecord);

        void updateTodoPagoAddressBookRecord(TodoPagoAddressBookRecord todoPagoAddressBookRecord);
    }
}
