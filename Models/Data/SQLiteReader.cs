
using System;

namespace Models
{
    public class SQLiteReader : FileReader
    {
        public override string Extension
        {
            get
            {
                return "Budget files (*.sbdb)|*.sbdb";
            }
        }

        public override bool InitializeFile(string fileName)
        {
            try
            {
                // TODO
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
                // TODO
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
