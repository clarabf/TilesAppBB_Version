﻿using SQLite;
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
        readonly SQLiteAsyncConnection _database;

        public LocalDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            try
            {
                _database.CreateTableAsync<User>().Wait();
                _database.CreateTableAsync<ConfigFile>().Wait();
                _database.CreateTableAsync<UserApp>().Wait();
                _database.CreateTableAsync<PendingOperation>().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #region USER METHODS
        public Task<List<User>> GetUsersAsync()
        {
            return _database.Table<User>().ToListAsync();
        }

        public Task<User> GetUserAsync(int id)
        {
            return _database.Table<User>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }
        public Task<User> GetLastLoggedInUserAsync()
        {
            return _database.Table<User>().OrderByDescending(u => u.LastLogIn).FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(User User)
        {
            if (User.Id != 0)
            {
                return _database.UpdateAsync(User);
            }
            else
            {
                return _database.InsertAsync(User);
            }
        }

        public Task<int> DeleteUserAsync(User User)
        {
            return _database.DeleteAsync(User);
        }

        public Task<int> DeleteAllUsersAsync() {
            return _database.DropTableAsync<User>();
        }
        #endregion

        #region CONFIGURATIONS FILES METHODS
        public async Task<List<ConfigFile>> GetUserConfigFilesAsync(int userId)
        {
            List<UserApp> userApps =await GetUserAppsAsync(userId);
            List<int> userAppsIds = new List<int>();
            foreach (var item in userApps)
            {
                userAppsIds.Add(item.ConfigFileId);
            }
            foreach (var item in userApps)
            {

            }
            return await _database.Table<ConfigFile>().Where(i => userAppsIds.Contains( i.Id)).ToListAsync();
        }

        public Task<ConfigFile> GetConfigFileAsync(int id)
        {
            return _database.Table<ConfigFile>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveConfigFileAsync(ConfigFile ConfigFile)
        {
            if (ConfigFile.Id != 0)
            {
                return _database.UpdateAsync(ConfigFile);
            }
            else
            {
                return _database.InsertAsync(ConfigFile);
            }
        }

        public Task<int> DeleteConfigFileAsync(ConfigFile ConfigFile)
        {
            return _database.DeleteAsync(ConfigFile);
        }
        public Task<int> DeleteAllConfigFilesAsync()
        {
            return _database.DropTableAsync<ConfigFile>();
        }
        #endregion

        #region USER APPS METHODS
        public Task<List<UserApp>> GetUserAppsAsync(int userId)
        {
            return _database.Table<UserApp>()
                            .Where(i => i.UserId == userId).
                            ToListAsync();
        }
        public Task<UserApp> GetUserAppsByConfigFileIdAndUserIdAsync(int ConfigFileId,int UserId)
        {
            return _database.Table<UserApp>()
                            .Where(i => i.ConfigFileId == ConfigFileId && i.UserId == UserId).
                            FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAppAsync(UserApp UserApp)
        {
            if (UserApp.Id != 0)
            {
                return _database.UpdateAsync(UserApp);
            }
            else
            {
                return _database.InsertAsync(UserApp);
            }
        }

        public Task<int> DeleteUserAppAsync(UserApp UserApp)
        {
            return _database.DeleteAsync(UserApp);
        }
        public Task<int> DeleteUserAppsByConfigFileIdAndUserIdAsync(int ConfigFileId, int UserId)
        {
            return _database.DeleteAsync(GetUserAppsByConfigFileIdAndUserIdAsync(ConfigFileId,UserId));
        }
        public Task<int> DeleteAllUserAppsAsync()
        {
            return _database.DropTableAsync<UserApp>();
        }
        #endregion

        #region PENDING OPERATIONS METHODS
        public Task<List<PendingOperation>> GetPendingOperationsAsync()
        {
            return _database.Table<PendingOperation>().ToListAsync();
        }

        public Task<PendingOperation> GetPendingOperationAsync(int id)
        {
            return _database.Table<PendingOperation>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SavePendingOperationAsync(PendingOperation PendingOperation)
        {
            if (PendingOperation.Id != 0)
            {
                return _database.UpdateAsync(PendingOperation);
            }
            else
            {
                return _database.InsertAsync(PendingOperation);
            }
        }

        public Task<int> DeletePendingOperationAsync(PendingOperation PendingOperation)
        {
            return _database.DeleteAsync(PendingOperation);
        }
        public Task<int> DeleteAllPendingOperationsAsync()
        {
            return _database.DropTableAsync<PendingOperation>();
        }
        #endregion
    }
}