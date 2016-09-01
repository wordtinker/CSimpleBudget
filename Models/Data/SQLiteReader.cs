using System.Data.SQLite;
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
            string sql = "SELECT name, rowid FROM Categories ORDER BY name ASC";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    topCategories.Add(new Category { Name = dr.GetString(0), Parent = null, Id = dr.GetInt32(1) });
                }
                dr.Close();
            }
            // Get sucategories
            List<Category> categories = new List<Category>(topCategories);
            foreach (Category top in topCategories)
            {
                foreach (Category child in SelectSubcategoriesFor(top.Name))
                {
                    child.Parent = top;
                    categories.Add(child);
                    top.Children.Add(child);
                }
            }
            // Get empty top category
            categories.Add(new Category { Name = string.Empty, Parent = null });
            return categories;
        }

        private List<Category> SelectSubcategoriesFor(string parent)
        {
            List<Category> categories = new List<Category>();
            string sql = "SELECT name, rowid FROM Subcategories WHERE parent=@parent";
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
                    categories.Add(new Category { Name = dr.GetString(0), Id = dr.GetInt32(1) });
                }
                dr.Close();
            }
            return categories;
        }

        private bool ManageCategory(string sql, string name, out int rowid )
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
                rowid = Convert.ToInt32(connection.LastInsertRowId);
                return true;
            }
            catch (SQLiteException)
            {
                rowid = 0;
                return false;
            }
        }

        public override bool AddCategory(string name, string parent, out Category cat)
        {
            if (parent == string.Empty)
            {
                string sql = "INSERT INTO Categories VALUES(@name)";
                int rowid;
                if (ManageCategory(sql, name, out rowid))
                {
                    cat = new Category { Id = rowid, Name = name, Parent = null };
                    return true;
                }
                else
                {
                    cat = null;
                    return false;
                }
            }
            else
            {
                string sql = "INSERT INTO Subcategories VALUES(@name, @parent)";
                int rowid;
                if (ManageSubcategory(sql, name, parent, out rowid))
                {
                    // TODO !!! change string parent to object + add to children
                    cat = new Category { Id = rowid, Name = name, Parent = null };
                    return true;
                }
                else
                {
                    cat = null;
                    return false;
                }
            }
        }

        public override bool DeleteCategory(Category cat)
        {
            // Top category with no children
            if (cat.Parent == null && cat.Children.Count == 0)
            {
                string sql = "DELETE FROM Categories WHERE name=@name";
                int rowid;
                return ManageCategory(sql, cat.Name, out rowid);
            }
            else if (cat.Parent != null)
            {
                // Can't delete if there is a transaction or budget record
                if (ExistsTransaction(cat) || ExistsRecord(cat))
                {
                    return false;
                }
                else
                {
                    string sql = " DELETE FROM Subcategories WHERE rowid=@rowid";
                    try
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.Add(new SQLiteParameter()
                            {
                                ParameterName = "@rowid",
                                DbType = System.Data.DbType.Int32,
                                Value = cat.Id
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
            }
            else
            {   // Can't delete if there is a child category
                return false;
            }
        }

        private bool ManageSubcategory(string sql, string name, string parent, out int rowid)
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
                rowid = Convert.ToInt32(connection.LastInsertRowId);
                return true;
            }
            catch (SQLiteException)
            {
                rowid = 0;
                return false;
            }
        }

        /************** Accounts *****************/

        public override List<Account> SelectAccounts()
        {
            List<Account> accounts = new List<Account>();
            string sql = "SELECT *, rowid FROM Accounts";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    accounts.Add(new Account {
                        Name = dr.GetString(0),
                        Type = (AccType)Enum.Parse(typeof(AccType), dr.GetString(1)),
                        Balance = dr.GetDecimal(2)/100,
                        Closed = Convert.ToBoolean(dr.GetInt32(3)),
                        Excluded = Convert.ToBoolean(dr.GetInt32(4)),
                        Id = dr.GetInt32(5)
                    });
                }
                dr.Close();
            }
            return accounts;
        }

        public override bool AddAccount(string name, out Account acc)
        {
            string sql = "INSERT INTO Accounts VALUES(@name, @type, 0, 0, 0)";
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
                        ParameterName = "@type",
                        DbType = System.Data.DbType.String,
                        Value = AccType.Bank.ToString()
                    });
                    cmd.ExecuteNonQuery();
                }
                acc = new Account
                {
                    Name = name,
                    Type = AccType.Bank,
                    Closed = false,
                    Excluded = false,
                    Balance = decimal.Zero,
                    Id = Convert.ToInt32(connection.LastInsertRowId)
                };
                return true;
            }
            catch (SQLiteException)
            {
                acc = null;
                return false;
            }
        }

        public override void UpdateAccount(Account acc)
        {
            string sql = "UPDATE Accounts SET type=@type, balance=@balance, closed=@closed, " +
                "exbudget=@excluded WHERE rowid=@rowid";
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@type",
                        DbType = System.Data.DbType.String,
                        Value = acc.Type.ToString()
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@balance",
                        DbType = System.Data.DbType.Int32,
                        Value = decimal.ToInt32(acc.Balance * 100)
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@closed",
                        DbType = System.Data.DbType.Int32,
                        Value = Convert.ToInt32(acc.Closed)
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@excluded",
                        DbType = System.Data.DbType.Int32,
                        Value = Convert.ToInt32(acc.Excluded)
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@rowid",
                        DbType = System.Data.DbType.Int32,
                        Value = acc.Id
                    });
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SQLiteException)
            {
                //
            }
        }

        public override bool DeleteAccount(Account acc)
        {
            // Cant delete if there is a transaction on account
            if (ExistsTransaction(acc))
            {
                return false;
            }

            string sql = "DELETE FROM Accounts WHERE rowid=@rowid";
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@rowid",
                        DbType = System.Data.DbType.Int32,
                        Value = acc.Id
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

        /************** Transactions *****************/

        public override List<Transaction> SelectTransactions(Account acc)
        {
            List<Transaction> transactions = new List<Transaction>();
            string sql = "SELECT date, amount, info, category_id, rowid FROM Transactions " +
                         "WHERE acc_id=@id ORDER BY date DESC";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@id",
                    DbType = System.Data.DbType.Int32,
                    Value = acc.Id
                });
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    transactions.Add(new Transaction
                    {
                        Date = dr.GetDateTime(0),
                        Value = dr.GetDecimal(1)/100,
                        Info = dr.GetString(2),
                        CategoryId = dr.GetInt32(3),
                        Id = dr.GetInt32(4)
                    });
                }
                dr.Close();
            }
            return transactions;
        }

        private bool ExistsTransaction(Account acc)
        {
            string sql = "SELECT COUNT(*) FROM Transactions WHERE acc_id=@rowid";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@rowid",
                    DbType = System.Data.DbType.Int32,
                    Value = acc.Id
                });
                Int32 count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private bool ExistsTransaction(Category cat)
        {
            string sql = "SELECT COUNT(*) FROM Transactions WHERE category_id=@catid";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@catid",
                    DbType = System.Data.DbType.Int32,
                    Value = cat.Id
                });
                Int32 count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        /************** Records *****************/

        private bool ExistsRecord(Category cat)
        {
            string sql = "SELECT COUNT(*) FROM Budget WHERE category_id=@catid";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@catid",
                    DbType = System.Data.DbType.Int32,
                    Value = cat.Id
                });
                Int32 count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        /************** File *****************/

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
