using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Payments.TodoPago.Domain;


namespace Nop.Plugin.Payments.TodoPago.Services
{
    public partial class TodoPagoTransactionService : ITodoPagoTransactionService {
       
        private const string TODOPAGO_ALL_KEY = "Nop.todoPago.all-{0}-{1}";
        private const string TODOPAGO_PATTERN_KEY = "Nop.todoPago.";

        private readonly IRepository<TodoPagoTransactionRecord> _todoPagoTransactionRecordRepository;
        private readonly ICacheManager _cacheManager;


        public TodoPagoTransactionService(ICacheManager cacheManager, IRepository<TodoPagoTransactionRecord> todoPagoTransactionRecordRepository) {
            this._cacheManager = cacheManager;
            this._todoPagoTransactionRecordRepository = todoPagoTransactionRecordRepository;
        }


        public virtual void deleteTodoPagoTransactionRecord(TodoPagoTransactionRecord todoPagoTransactionRecord){

            if (todoPagoTransactionRecord == null) {
                throw new ArgumentNullException("todoPagoTransactionRecord");
            }

            _todoPagoTransactionRecordRepository.Delete(todoPagoTransactionRecord);
            _cacheManager.RemoveByPattern(TODOPAGO_PATTERN_KEY);
        }

        public virtual IPagedList<TodoPagoTransactionRecord> findAll(int pageIndex = 0, int pageSize = int.MaxValue)
       {
            string key = string.Format(TODOPAGO_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
           {
                var query = from tpt in _todoPagoTransactionRecordRepository.Table
                            orderby tpt.ordenId, tpt.firstStep, tpt.paramsSAR, tpt.responseSAR, tpt.secondStep, tpt.paramsGAA,tpt.responseGAA,tpt.requestKey,tpt.publicRequestKey,tpt.answerKey
                            select tpt;


                var records = new PagedList<TodoPagoTransactionRecord>(query, pageIndex, pageSize);
               return records;
           });
        }

        public virtual TodoPagoTransactionRecord findById(int Id){

            if (Id == 0){
                return null;
            }

            return _todoPagoTransactionRecordRepository.GetById(Id);
        }

        public virtual TodoPagoTransactionRecord findByOrdenId(int Id) {

            if (Id == 0){
                return null;
            }

            //filter by ordenId
            //var result = this.findAll().Where(tp => tp.ordenId == ordenId).ToList();

            var query = from tpt in _todoPagoTransactionRecordRepository.Table
                        orderby tpt.ordenId, tpt.firstStep, tpt.paramsSAR, tpt.responseSAR, tpt.secondStep, tpt.paramsGAA, tpt.responseGAA, tpt.requestKey, tpt.publicRequestKey, tpt.answerKey
                        where tpt.ordenId == Id
                        select tpt;

            var result = query.FirstOrDefault();
        
            return result;
        }

        public virtual void insertTodoPagoTransactionRecord(TodoPagoTransactionRecord todoPagoTransactionRecord) {

            if (todoPagoTransactionRecord == null) {
                throw new ArgumentNullException("todoPagoTransactionRecord");
            }

            _todoPagoTransactionRecordRepository.Insert(todoPagoTransactionRecord);
            _cacheManager.RemoveByPattern(TODOPAGO_PATTERN_KEY);
        }

        public virtual void updateTodoPagoTransactionRecord(TodoPagoTransactionRecord todoPagoTransactionRecord) {

            if (todoPagoTransactionRecord == null) {
                throw new ArgumentNullException("todoPagoTransactionRecord");
            }

            _todoPagoTransactionRecordRepository.Update(todoPagoTransactionRecord);
            _cacheManager.RemoveByPattern(TODOPAGO_PATTERN_KEY);
        }


    }
}
