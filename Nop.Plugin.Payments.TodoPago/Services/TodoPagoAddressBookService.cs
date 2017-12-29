using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Payments.TodoPago.Domain;

namespace Nop.Plugin.Payments.TodoPago.Services
{
    public partial class TodoPagoAddressBookService : ITodoPagoAddressBookService
    {
        private const string TODOPAGO_ALL_KEY = "Nop.todoPago.all-{0}-{1}";
        private const string TODOPAGO_PATTERN_KEY = "Nop.todoPago.";

        private readonly IRepository<TodoPagoAddressBookRecord> _todoPagoAddressBookRecordRepository;
        private readonly ICacheManager _cacheManager;

        public TodoPagoAddressBookService(ICacheManager cacheManager, IRepository<TodoPagoAddressBookRecord> todoPagoAddressBookRecordRepository)
        {
            this._cacheManager = cacheManager;
            this._todoPagoAddressBookRecordRepository = todoPagoAddressBookRecordRepository;
        }

        public void deleteTodoPagoAddressBookRecord(TodoPagoAddressBookRecord todoPagoAddressBookRecord)
        {
            if (todoPagoAddressBookRecord == null)
                throw new ArgumentNullException("todoPagoAddressBookRecord");

            _todoPagoAddressBookRecordRepository.Delete(todoPagoAddressBookRecord);
            _cacheManager.RemoveByPattern(TODOPAGO_PATTERN_KEY);
        }

        public IPagedList<TodoPagoAddressBookRecord> findAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(TODOPAGO_ALL_KEY, pageIndex, pageSize);

            return _cacheManager.Get(key, () =>
            {
                var query = from tpab in _todoPagoAddressBookRecordRepository.Table
                            orderby tpab.hash, tpab.street, tpab.state, tpab.city, tpab.country, tpab.postal
                            select tpab;
                
                var records = new PagedList<TodoPagoAddressBookRecord>(query, pageIndex, pageSize);

                return records;
            });
        }

        public TodoPagoAddressBookRecord findById(int todoPagoTransactionRecordId)
        {
            if (todoPagoTransactionRecordId == 0)
                return null;

            return _todoPagoAddressBookRecordRepository.GetById(todoPagoTransactionRecordId);
        }

        public TodoPagoAddressBookRecord findByHash(string hash)
        {
            if (String.IsNullOrEmpty(hash))
                return null;

            //filter by ordenId
            //var result = this.findAll().Where(tp => tp.ordenId == ordenId).ToList();

            var query = from tpab in _todoPagoAddressBookRecordRepository.Table
                        orderby tpab.hash, tpab.street, tpab.state, tpab.city, tpab.country, tpab.postal
                        where tpab.hash == hash
                        select tpab;

            var result = query.FirstOrDefault();

            return result;
        }

        public void insertTodoPagoAddressBookRecord(TodoPagoAddressBookRecord todoPagoAddressBookRecord)
        {
            if (todoPagoAddressBookRecord == null)
                throw new ArgumentNullException("todoPagoAddressBookRecord");

            _todoPagoAddressBookRecordRepository.Insert(todoPagoAddressBookRecord);
            _cacheManager.RemoveByPattern(TODOPAGO_PATTERN_KEY);
        }

        public void updateTodoPagoAddressBookRecord(TodoPagoAddressBookRecord todoPagoAddressBookRecord)
        {
            if (todoPagoAddressBookRecord == null)
                throw new ArgumentNullException("todoPagoAddressBookRecord");

            _todoPagoAddressBookRecordRepository.Update(todoPagoAddressBookRecord);
            _cacheManager.RemoveByPattern(TODOPAGO_PATTERN_KEY);
        }
    }
}
