﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using PM2E13549.Models;
using System.Threading.Tasks;
using System.Data.Common;

namespace PM2E13549.Controllers
{
    public class DBProc
    {
        readonly SQLiteAsyncConnection _connection;
        public DBProc() { }
        public DBProc(string dbpath)
        {
            _connection = new SQLiteAsyncConnection(dbpath);
            /*Crear todos mis objetos de base de datos: tablas*/
            _connection.CreateTableAsync<Sitios>().Wait();
        }
        /*Crear el CRUD de BD*/
        //Create
        public Task<int> AddSitio(Sitios sitios)
        {
            if (sitios.Id == 0)
            {
                return _connection.InsertAsync(sitios);
            }
            else
            {
                return _connection.UpdateAsync(sitios);
            }
        }

        //Read
        public Task<List<Sitios>> GetAll()
        {
            return _connection.Table<Sitios>().ToListAsync();
        }

        public Task<Sitios> GetById(int id)
        {
            return _connection.Table<Sitios>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        //Delete
        public Task<int> DeleteSitio(Sitios sitios)
        {
            return _connection.DeleteAsync(sitios);
        }
    }
}
