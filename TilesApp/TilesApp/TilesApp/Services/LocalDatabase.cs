using SQLite;
using System;
using System.Collections.Generic;
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
                _database.CreateTable<ConfigFile>();
                _database.CreateTable<UserApp>();
                _database.CreateTable<PendingOperation>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #region USER METHODS
        public List<User> GetUsers()
        {
            return _database.Table<User>().ToList();
        }

        public User GetUser(int id)
        {
            return _database.Table<User>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }
        public User GetUser(string email, string password)
        {
            return _database.Table<User>()
                            .Where(i => i.Email == email && i.Password == password)
                            .FirstOrDefault();
        }
        public User GetLastLoggedInUser()
        {
            return _database.Table<User>().OrderByDescending(u => u.LastLogIn).FirstOrDefault();
        }

        public int SaveUser(User User)
        {
            if (User.Id != 0)
            {
                return _database.Update(User);
            }
            else
            {
                return _database.Insert(User);
            }
        }

        public int DeleteUser(User User)
        {
            return _database.Delete(User);
        }

        public int DeleteAllUsers() {
            return _database.DeleteAll<User>();
        }
        #endregion

        #region CONFIGURATIONS FILES METHODS
        public List<ConfigFile> GetUserConfigFiles(int userId)
        {
            List<UserApp> userApps = GetUserApps(userId);
            List<int> userAppsIds = new List<int>();
            foreach (var item in userApps)
            {
                userAppsIds.Add(item.ConfigFileId);
            }
            foreach (var item in userApps)
            {

            }
            return  _database.Table<ConfigFile>().Where(i => userAppsIds.Contains( i.Id)).ToList();
        }

        public ConfigFile GetConfigFile(int id)
        {
            return _database.Table<ConfigFile>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }

        public int SaveConfigFile(ConfigFile ConfigFile)
        {
            if (ConfigFile.Id != 0)
            {
                  _database.Update(ConfigFile);
                return -1;
            }
            else
            {
                 _database.Insert(ConfigFile);
                return ConfigFile.Id;
            }
        }

        public int DeleteConfigFile(ConfigFile ConfigFile)
        {
            return _database.Delete(ConfigFile);
        }
        public int DeleteAllConfigFiles()
        {
            return _database.DeleteAll<ConfigFile>();
        }
        #endregion

        #region USER APPS METHODS
        public List<UserApp> GetUserApps(int userId)
        {
            return _database.Table<UserApp>()
                            .Where(i => i.UserId == userId).
                            ToList();
        }
        public UserApp GetUserAppByConfigFileIdAndUserId(int ConfigFileId,int UserId)
        {
            return _database.Table<UserApp>()
                            .Where(i => i.ConfigFileId == ConfigFileId && i.UserId == UserId).
                            FirstOrDefault();
        }
        public List<UserApp> GetUserAppsByUserId(int UserId)
        {
            return _database.Table<UserApp>()
                            .Where(i => i.UserId == UserId).ToList();
        }

        public int SaveUserApp(UserApp UserApp)
        {
            if (UserApp.Id != 0)
            {
                return _database.Update(UserApp);
            }
            else
            {
                return _database.Insert(UserApp);
            }
        }

        public int DeleteUserApp(UserApp UserApp)
        {
            return _database.Delete(UserApp);
        }
        public int DeleteUserAppsByConfigFileIdAndUserId(int ConfigFileId, int UserId)
        {
            return _database.Delete(GetUserAppByConfigFileIdAndUserId(ConfigFileId,UserId));
        }
        public int DeleteAllUserApps(int userId = -1)
        {
            if (userId == -1) return _database.DeleteAll<UserApp>();
            else
            {
                try
                {
                    var tempList = GetUserAppsByUserId(userId);
                    foreach (UserApp ua in tempList)
                    {
                        DeleteUserApp(ua);
                    }
                    return 1;
                }
                catch 
                {
                    return -1;
                }
            }
       }
        #endregion

        #region PENDING OPERATIONS METHODS
        public List<PendingOperation> GetPendingOperations()
        {
            return _database.Table<PendingOperation>().ToList();
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

        #region
        public void DeleteDatabase()
        {
            DeleteAllUsers();
            DeleteAllUserApps();
            DeleteAllConfigFiles();
        }
        #endregion
    }
}
