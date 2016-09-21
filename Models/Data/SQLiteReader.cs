using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class SQLiteReader : FileReader
    {
        private SQLiteConnection connection;
        private string connString = "Data Source={0};Version=3;foreign keys=True;";

        public override string Extension { get; } = "Budget files (*.sbdb)|*.sbdb";

        private static decimal FromDBValToDecimal(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(decimal);
            }
            else
            {
                return Convert.ToDecimal(obj) / 100;
            }
        }

        private static int FromDecimaltoDBInt(decimal value)
        {
            return decimal.ToInt32(value * 100);
        }

        /************** Categories *****************/
        internal override List<Category> SelectCategories()
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
            // Get subcategories
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

        internal override bool AddCategory(string name, Category parent, out Category cat)
        {
            // Can't add empty name 
            if (name == string.Empty)
            {
                cat = null;
                return false;
            }

            if (parent == null)
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
                if (ManageSubcategory(sql, name, parent.Name, out rowid))
                {
                    cat = new Category { Id = rowid, Name = name, Parent = parent };
                    parent.Children.Add(cat);
                    return true;
                }
                else
                {
                    cat = null;
                    return false;
                }
            }
        }

        internal override bool DeleteCategory(Category cat)
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

        internal override List<Account> SelectAccounts()
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
                        Type = dr.GetString(1),
                        Balance = FromDBValToDecimal(dr.GetDecimal(2)),
                        Closed = Convert.ToBoolean(dr.GetInt32(3)),
                        Excluded = Convert.ToBoolean(dr.GetInt32(4)),
                        Id = dr.GetInt32(5)
                    });
                }
                dr.Close();
            }
            return accounts;
        }

        internal override bool AddAccount(string name, string accType, out Account acc)
        {
            // Can't add empty account name
            if (name == string.Empty)
            {
                acc = null;
                return false;
            }

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
                        Value = accType
                    });
                    cmd.ExecuteNonQuery();
                }
                acc = new Account
                {
                    Name = name,
                    Type = accType,
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

        internal override bool UpdateAccount(Account acc)
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
                        Value = acc.Type
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@balance",
                        DbType = System.Data.DbType.Int32,
                        Value = FromDecimaltoDBInt(acc.Balance)
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
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        internal override bool DeleteAccount(Account acc)
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

        private void UpdateTotal(Account acc)
        {
            Decimal total;
            string sql = "SELECT SUM(amount) FROM Transactions WHERE acc_id=@id";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@id",
                    DbType = System.Data.DbType.Int32,
                    Value = acc.Id
                });
                total = FromDBValToDecimal(cmd.ExecuteScalar());
            }
            acc.Balance = total;
            // save new total to DB
            UpdateAccount(acc);
        }

        /************** Transactions *****************/

        private Category GetCategoryForId(int catId)
        {
            return (from cat in Core.Instance.Categories
            where cat.Id == catId && cat.Parent != null
            select cat).First();
        }

        internal override List<Transaction> SelectTransactions(Account acc)
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
                    transactions.Add(new Transaction(dr.GetInt32(4), acc)
                    {
                        Date = dr.GetDateTime(0),
                        Amount = FromDBValToDecimal(dr.GetDecimal(1)),
                        Info = dr.GetString(2),
                        Category = GetCategoryForId(dr.GetInt32(3))
                    });
                }
                dr.Close();
            }
            return transactions;
        }

        internal override decimal SelectTransactionsCombined(int currentYear, int currentMonth, Category cat)
        {
            DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);
            DateTime lastDayofPrevMonth = firstDayOfMonth.AddSeconds(-1);

            // BETWEEN firstDay and lastDay is glitchy
            // have to use > and <=
            string sql = @"SELECT sum(t.amount) FROM Transactions as t
                           INNER JOIN Accounts as a
                           on t.acc_id = a.rowid
                           WHERE date>@startDate and date<=@endDate
                           AND category_id=@catId
                           AND exbudget = 0";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@catId",
                    DbType = System.Data.DbType.Int32,
                    Value = cat.Id
                });
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@startDate",
                    DbType = System.Data.DbType.Date,
                    Value = lastDayofPrevMonth
            });
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@endDate",
                    DbType = System.Data.DbType.Date,
                    Value = lastDayOfMonth
                });
                return FromDBValToDecimal(cmd.ExecuteScalar());
            }
        }

        internal override bool DeleteTransaction(Transaction transaction)
        {
            string sql = "DELETE FROM Transactions WHERE rowid=@id";
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@id",
                        DbType = System.Data.DbType.Int32,
                        Value = transaction.Id
                    });
                    cmd.ExecuteNonQuery();
                }
                UpdateTotal(transaction.Account);
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        internal override bool AddTransaction(
            Account currentAccount, DateTime date, decimal amount, string info, Category category, out Transaction newTr)
        {
            string sql = "INSERT INTO Transactions VALUES(@date, @amount, @info, @accId, @catId)";
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@date",
                        DbType = System.Data.DbType.Date,
                        Value = date.Date
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@amount",
                        DbType = System.Data.DbType.Int32,
                        Value = FromDecimaltoDBInt(amount)
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@info",
                        DbType = System.Data.DbType.String,
                        Value = info ?? string.Empty
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@accId",
                        DbType = System.Data.DbType.Int32,
                        Value = currentAccount.Id
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@catId",
                        DbType = System.Data.DbType.Int32,
                        Value = category.Id
                    });
                    cmd.ExecuteNonQuery();
                }
                newTr = new Transaction(Convert.ToInt32(connection.LastInsertRowId), currentAccount)
                {
                    Amount = amount,
                    Date = date,
                    Info = info,
                    Category = category
                };
                UpdateTotal(currentAccount);
                return true;
            }
            catch (SQLiteException)
            {
                newTr = null;
                return false;
            }
        }

        internal override bool UpdateTransaction(Transaction tr, DateTime date, decimal amount, string info, Category category)
        {
            string sql = "UPDATE Transactions SET date=@date, amount=@amount, info=@info, category_id=@catId WHERE rowid=@rowId";
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@date",
                        DbType = System.Data.DbType.Date,
                        Value = date.Date
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@amount",
                        DbType = System.Data.DbType.Int32,
                        Value = FromDecimaltoDBInt(amount)
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@info",
                        DbType = System.Data.DbType.String,
                        Value = info ?? string.Empty
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@catId",
                        DbType = System.Data.DbType.Int32,
                        Value = category.Id
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@rowId",
                        DbType = System.Data.DbType.Int32,
                        Value = tr.Id
                    });
                    cmd.ExecuteNonQuery();
                }
                tr.Category = category;
                tr.Date = date;
                tr.Info = info;
                tr.Amount = amount;
                UpdateTotal(tr.Account);
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
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

        internal override List<BudgetRecord> SelectRecords(int currentYear, int currentMonth)
        {
            string sql = "SELECT *, rowid FROM Budget WHERE month=@month AND year=@year";
            List<BudgetRecord> records = new List<BudgetRecord>();
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@month",
                    DbType = System.Data.DbType.Int32,
                    Value = currentMonth
                });
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@year",
                    DbType = System.Data.DbType.Int32,
                    Value = currentYear
                });
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BudgetType type;
                    Enum.TryParse<BudgetType>(dr.GetString(2), out type);
                    records.Add(new BudgetRecord(dr.GetInt32(6))
                    {
                        Amount = FromDBValToDecimal(dr.GetDecimal(0)),
                        Category = GetCategoryForId(dr.GetInt32(1)),
                        Type = type,
                        OnDay = dr.GetInt32(3),
                        Month = currentMonth,
                        Year = currentYear
                    });
                }
                dr.Close();
            }
            return records;
        }

        internal override decimal SelectRecordsCombined(int currentYear, int currentMonth, Category cat)
        {
            string sql = "SELECT sum(amount) FROM Budget WHERE month=@month AND year=@year AND category_id=@catId";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@catId",
                    DbType = System.Data.DbType.Int32,
                    Value = cat.Id
                });
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@month",
                    DbType = System.Data.DbType.Int32,
                    Value = currentMonth
                });
                cmd.Parameters.Add(new SQLiteParameter()
                {
                    ParameterName = "@year",
                    DbType = System.Data.DbType.Int32,
                    Value = currentYear
                });
                return FromDBValToDecimal(cmd.ExecuteScalar());
            }
        }

        internal override bool DeleteRecord(BudgetRecord record)
        {
            string sql = "DELETE FROM Budget WHERE rowid=@rowid";
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@rowid",
                        DbType = System.Data.DbType.Int32,
                        Value = record.Id
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

        internal override bool AddRecord(
            decimal amount, Category category, BudgetType budgetType, int onDay,
            int selectedMonth, int selectedYear, out BudgetRecord newRecord)
        {
            string sql = "INSERT INTO Budget VALUES(@amount, @catId, @btype, @onDay, @year, @month)";
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@amount",
                        DbType = System.Data.DbType.Int32,
                        Value = FromDecimaltoDBInt(amount)
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@catId",
                        DbType = System.Data.DbType.Int32,
                        Value = category.Id
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@btype",
                        DbType = System.Data.DbType.String,
                        Value = budgetType.ToString()
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@onDay",
                        DbType = System.Data.DbType.Int32,
                        Value = onDay
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@month",
                        DbType = System.Data.DbType.Int32,
                        Value = selectedMonth
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@year",
                        DbType = System.Data.DbType.Int32,
                        Value = selectedYear
                    });
                    cmd.ExecuteNonQuery();
                }
                newRecord = new BudgetRecord(Convert.ToInt32(connection.LastInsertRowId))
                {
                    Amount = amount,
                    Category = category,
                    Month = selectedMonth,
                    Year = selectedYear,
                    OnDay = onDay,
                    Type = budgetType
                };
                return true;
            }
            catch (SQLiteException)
            {
                newRecord = null;
                return false;
            }
        }

        internal override bool UpdateRecord(
            BudgetRecord record, decimal amount, Category category, BudgetType budgetType,
            int onDay, int selectedMonth, int selectedYear)
        {
            string sql = "UPDATE Budget SET amount=@amount, category_id=@catId, type=@btype, day=@onDay, year=@year, month=@month WHERE rowid=@rowid";
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@amount",
                        DbType = System.Data.DbType.Int32,
                        Value = FromDecimaltoDBInt(amount)
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@catId",
                        DbType = System.Data.DbType.Int32,
                        Value = category.Id
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@btype",
                        DbType = System.Data.DbType.String,
                        Value = budgetType.ToString()
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@onDay",
                        DbType = System.Data.DbType.Int32,
                        Value = onDay
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@month",
                        DbType = System.Data.DbType.Int32,
                        Value = selectedMonth
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@year",
                        DbType = System.Data.DbType.Int32,
                        Value = selectedYear
                    });
                    cmd.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@rowid",
                        DbType = System.Data.DbType.Int32,
                        Value = record.Id
                    });
                    cmd.ExecuteNonQuery();
                }
                record.Amount = amount;
                record.Category = category;
                record.Type = budgetType;
                record.OnDay = onDay;
                record.Month = selectedMonth;
                record.Year = selectedYear;
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        /************** Misc *****************/

        internal override int? GetMaximumYear()
        {
            string sql = "SELECT MAX(year) FROM Budget";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                object toConvert = cmd.ExecuteScalar();
                if (Convert.IsDBNull(toConvert))
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt32(toConvert);
                }
            }
        }

        internal override int? GetMinimumYear()
        {
            string sql = "SELECT MIN(year) FROM Budget";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                object toConvert = cmd.ExecuteScalar();
                if (Convert.IsDBNull(toConvert))
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt32(toConvert);
                }
            }
        }

        /************** File *****************/

        public override bool InitializeFile(string fileName)
        {
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
                    "category_id INTEGER, type TEXT, day INTEGER, year INTEGER, month INTEGER)";
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
