using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Xml;


namespace StorageCore.Utils
{
    public class DataBaseInitiaalizer
    {
        private string _dbName = "DataBase.sqlite";
        private SQLiteConnection _conn;

        public void Initialize()
        {
            //Create DataBase if not exist
            if (!File.Exists(_dbName))
            {
                SQLiteConnection.CreateFile(_dbName);
            }

            //Connection
            _conn = new SQLiteConnection($"Data Source = {_dbName}; Version = 3");
            _conn.Open();

            //Create tables 
            CreateEmployeesTable();
            CreateRolesTable();
            CreateMenuTable();
            CreateEmployeeRoleTable();
            CreateRoleMenuTable();
        }

        // Tabla Employees
        private void CreateEmployeesTable()
        {
            string checkTableSql = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Employees';";
            if (Convert.ToInt32(new SQLiteCommand(checkTableSql, _conn).ExecuteScalar()) == 0)
            {
                string createSql = @"
                CREATE TABLE Employees (
                    Id_employee NVARCHAR(50) PRIMARY KEY,               -- PK
                    UserName NVARCHAR(50) NOT NULL UNIQUE,     -- unique
                    PasswordH NVARCHAR(200) NOT NULL,          -- Password hash
                    Name NVARCHAR(100),                        -- Real name
                    Email NVARCHAR(100),                       -- Email
                    Phone NVARCHAR(20),                        -- Phone
                    Status INT DEFAULT 1,                      -- Status (1-active, 0-disabled)
                    CreateTime TIMESTAMP DEFAULT (datetime('now', 'localtime')),  -- Create time
                    UpdateTime TIMESTAMP,                      -- Update time
                )";
                new SQLiteCommand(createSql, _conn).ExecuteNonQuery();
                Console.WriteLine("Employees table created successfully");
            }
        }

        // Tabla Roles
        private void CreateRolesTable()
        {
            string checkTableSql = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Roles';";
            if (Convert.ToInt32(new SQLiteCommand( checkTableSql, _conn).ExecuteScalar()) == 0)
            {
                string createSql = @"
                CREATE TABLE Roles (
                    Id_role NVARCHAR (50) PRIMARY KEY,
                    RoleName NVARCHAR (50) NOT NULL UNIQUE,     --Role name
                    Description NVARCHAR （200），               --Description
                    CreateTime TIMESTAMP DEFAULT (datetime('now','localtime'))  --Time
                    UpdateTime TIMESTAMP                        --Update time
                )";
                new SQLiteCommand(createSql,_conn).ExecuteNonQuery();
                Console.WriteLine("Roles table created successfuly");
            }
        }

        // Tabla Menu
        private void CreateMenuTable()
        {
            string checkTableSql = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Menu';";
            if(Convert.ToInt32(new SQLiteCommand(checkTableSql,_conn).ExecuteScalar()) == 0)
            {
                string createSql = @"
                    CREATE TABLE Menu(
                    Id_menu NVARCHAR (50) PRIMARY KEY,
                    ParentID NVARCHAR (50),
                    MenuName NVARCHAR(100) NOT NULL,           -- Menu name
                    Path NVARCHAR(200),                        -- Route path
                    Component NVARCHAR(200),                   -- Component path
                    Icon NVARCHAR(50),                         -- Menu icon
                    Sort INT DEFAULT 0,                        -- 排序号 / Sort order
                    IsVisible INT DEFAULT 1,                   -- Is visible (1-yes, 0-no)
                    CreateTime TIMESTAMP DEFAULT (datetime('now', 'localtime')),  -- Create time
                    UpdateTime TIMESTAMP,                      -- Update time
                )";
                new SQLiteCommand(createSql,_conn).ExecuteNonQuery();
                Console.WriteLine("Menu table created successfuly");
            }
        }

        // Tabla Employees Role
        private void CreateEmployeeRoleTable()
        {
            string checkTableSql = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='EmployeesRole';";
            if( Convert.ToInt32(new SQLiteCommand(checkTableSql , _conn).ExecuteScalar())== 0)
            {
                string createSql = @"
                CREATE TABLE EmployeesRole (
                    ID NVARCHAR(50) PRIMARY KEY,                    -- Relationship ID (primary key)
                    Id_employee NVARCHAR(50) NOT NULL,              -- User ID
                    Id_role NVARCHAR(50) NOT NULL,                  -- Role ID
                    CreateTime TIMESTAMP DEFAULT (datetime('now', 'localtime')),  -- Create time
                    UNIQUE(Id_employee, Id_role),                    -- Unique constraint
                    FOREIGN KEY (Id_employee) REFERENCES Employee(Id_employee) ON DELETE CASCADE,  -- Cascade delete
                    FOREIGN KEY (Id_role) REFERENCES Roles(Id_role) ON DELETE CASCADE
                )";
                new SQLiteCommand(createSql , _conn).ExecuteNonQuery();
                Console.WriteLine("Employees Role table created successfuly");
            }
        }

        // Tabla Role Menu
        private void CreateRoleMenuTable()
        {
            string checkTableSql = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='RoleMeu';";
            if(Convert.ToInt32(new SQLiteCommand(checkTableSql, _conn).ExecuteScalar()) == 0)
            {
                string createsql = @"
                CREATE TABLE RoleMenu (
                    ID NVARCHAR(50) PRIMARY KEY,               -- Relationship ID (primary key)
                    Id_role NVARCHAR(50) NOT NULL,              -- Role ID
                    Id_menu NVARCHAR(50) NOT NULL,              -- Menu ID
                    CreateTime TIMESTAMP DEFAULT (datetime('now', 'localtime')),  -- Create time
                    UNIQUE(Id_role, Id_menu),                    -- Unique constraint
                    FOREIGN KEY (Id_role) REFERENCES Roles(Id_role) ON DELETE CASCADE,  -- Cascade delete
                    FOREIGN KEY (Id_menu) REFERENCES Menu(Id_role) ON DELETE CASCADE
                )";
                new SQLiteCommand(createsql,_conn).ExecuteNonQuery();
                Console.WriteLine("Role Menu table created successfuly");
            }
        }
    }

   
}
