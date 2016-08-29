﻿using System.Data.SQLite;
using System;
using System.Collections.Generic;

namespace Models
{
    public class SQLiteReader : FileReader
    {
        private SQLiteConnection connection;
        private string connString = "Data Source={0};Version=3;foreign keys=True;";

        public override string Extension { get; } = "Budget files (*.sbdb)|*.sbdb";

        /************** Categories *****************/
        public override List<Category> SelectCategories()
        {
            // Get top categories
            List<Category> topCategories = new List<Category>();
            string sql = "SELECT name FROM Categories ORDER BY name ASC";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    topCategories.Add(new Category { Name = dr.GetString(0), Parent = null });
                }
                dr.Close();
            }
            // Get sucategories
            List<Category> categories = new List<Category>(topCategories);
            foreach (Category top in topCategories)
            {
                foreach (string name in SelectSubcategoriesFor(top.Name))
                {
                    Category item = new Category { Name = name, Parent = top };
                    categories.Add(item);
                    top.Children.Add(item);
                }
            }
            // Get empty top category
            categories.Add(new Category { Name = string.Empty, Parent = null });
            return categories;
        }

        private List<string> SelectSubcategoriesFor(string parent)
        {
            List<string> categories = new List<string>();
            string sql = "SELECT name FROM Subcategories WHERE parent=@parent";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@parent",
                    DbType =System.Data.DbType.String,
                    Value = parent
                });
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    categories.Add(dr.GetString(0));
                }
                dr.Close();
            }
            return categories;
        }

        private bool ManageCategory(string sql, string name)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@name",
                        DbType = System.Data.DbType.String,
                        Value = name
                    });
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        public override bool AddCategory(string name, string parent)
        {
            if (parent == string.Empty)
            {
                string sql = "INSERT INTO Categories VALUES(@name)";
                return ManageCategory(sql, name);
            }
            else
            {
                string sql = "INSERT INTO Subcategories VALUES(@name, @parent)";
                return ManageSubcategory(sql, name, parent);
            }
        }

        public override bool DeleteCategory(Category cat)
        {
            // Top category with no children
            if (cat.Parent == null && cat.Children.Count == 0)
            {
                string sql = "DELETE FROM Categories WHERE name=@name";
                return ManageCategory(sql, cat.Name);
            }
            else if (cat.Parent != null)
            {
                // TODO
                //# Can't delete if there is a transaction or budget record
                //            if (self.exists_transaction_for_category(category_id) or
                //                self.exists_record_for_category(category_id)):
                //            return False

                // TODO
                string sql = "";
                throw new NotImplementedException();
            }
            else
            {   // Can't delete if there is a child category
                return false;
            }
        }

        // TODO drop obsolete
        //private bool ExistsSubcategoryFor(string parent)
        //{
        //    string sql = "SELECT COUNT(*) FROM Subcategories WHERE parent=@parent";
        //    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
        //    {
        //        cmd.Parameters.Add(new SQLiteParameter()
        //        {
        //            ParameterName = "@parent",
        //            DbType = System.Data.DbType.String,
        //            Value = parent
        //        });
        //        Int32 count = Convert.ToInt32(cmd.ExecuteScalar());
        //        return count > 0;
        //    }
        //}

        private bool ManageSubcategory(string sql, string name, string parent)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@name",
                        DbType = System.Data.DbType.String,
                        Value = name
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@parent",
                        DbType = System.Data.DbType.String,
                        Value = parent
                    });
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        public override bool InitializeFile(string fileName)
        {
            // TOTO Later : move to foreign key support and cascade delete;
            try
            {
                string cString = string.Format(connString, fileName);
                SQLiteConnection dbConn = new SQLiteConnection(cString);
                dbConn.Open();
                
                string sql = "CREATE TABLE IF NOT EXISTS Accounts(name TEXT, " +
                    "type TEXT, balance INTEGER, closed INTEGER, exbudget INTEGER)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConn))
                {
                    cmd.ExecuteNonQuery();
                }

                sql = "CREATE TABLE IF NOT EXISTS Transactions(date DATE, " +
                    "amount INTEGER, info TEXT, acc_id INTEGER, category_id INTEGER)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConn))
                {
                    cmd.ExecuteNonQuery();
                }

                sql = "CREATE TABLE IF NOT EXISTS Categories(name TEXT UNIQUE)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConn))
                {
                    cmd.ExecuteNonQuery();
                }

                sql = "CREATE TABLE IF NOT EXISTS Subcategories(name TEXT, parent TEXT, UNIQUE(name, parent))";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConn))
                {
                    cmd.ExecuteNonQuery();
                }

                sql = "CREATE TABLE IF NOT EXISTS Budget(amount INTEGER, " +
                    "category_id INTEGER, type TEXT, day INTEGER, year INTEGER, month INTEGER";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConn))
                {
                    cmd.ExecuteNonQuery();
                }

                dbConn.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool LoadFile(string fileName)
        {
            try
            {
                string cString = string.Format(connString, fileName);
                connection = new SQLiteConnection(cString);
                connection.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}