using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models;
using TilesApp.Models.DataModels;

namespace TilesApp.Services
{
    public class LocalDatabase
    {
        public SQLiteConnection _database;

        public LocalDatabase(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            try
            {
                _database.CreateTable<User>();
                _database.CreateTable<PendingOperation>();
                _database.CreateTable<PrimitiveType>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #region PENDING OPERATIONS METHODS
        public List<PendingOperation> GetPendingOperations()
        {
            //Deleting forms after 90 days of creation.
            List<PendingOperation> expiredOps = _database.Table<PendingOperation>().ToList().FindAll(delegate (PendingOperation po) { return (DateTime.Now - po.CreatedAt).TotalDays > 90; });
            foreach (PendingOperation po in expiredOps) 
            {
                DeletePendingOperation(po);
            }
            return _database.Table<PendingOperation>().OrderByDescending(po => po.CreatedAt).ToList();
        }
        public PendingOperation GetPendingOperation(int id)
        {
            return _database.Table<PendingOperation>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }
        public PendingOperation GetFirstOfflineOperationInQueue()
        {
            return _database.Table<PendingOperation>().Where(i => i.OnOff == "Offline").OrderBy(u => u.CreatedAt).FirstOrDefault();
        }
        public int GetOfflineOperationsCount()
        {
            return _database.Table<PendingOperation>().ToList().FindAll(delegate (PendingOperation po) { return po.OnOff == "Offline"; }).Count;
        }
        public int SavePendingOperation(PendingOperation PendingOperation)
        {
            if (PendingOperation.Id != 0)
            {
                return _database.Update(PendingOperation);
            }
            else
            {
                return _database.Insert(PendingOperation);
            }
        }
        public int DeletePendingOperation(PendingOperation PendingOperation)
        {
            return _database.Delete(PendingOperation);
        }
        public int DeleteAllPendingOperations()
        {
            return _database.DeleteAll<PendingOperation>();
        }
        #endregion

        #region PRIMITIVE TYPES METHODS
        public List<PrimitiveType> GetPrimitiveTypes()
        {
            return _database.Table<PrimitiveType>().OrderBy(pt => pt.Id).ToList();
        }
        public int SavePrimitiveType(PrimitiveType pt)
        {
            return _database.Insert(pt);
        }
        public int DeleteAllPrimitiveTypes()
        {
            return _database.DeleteAll<PrimitiveType>();
        }
        #endregion

        #region GENERAL
        public void DeleteDatabase()
        {
            DeleteAllPendingOperations();
            DeleteAllPrimitiveTypes();
        }
        #endregion
    }
}
