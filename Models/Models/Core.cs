using Prism.Mvvm;
using System.Collections.Generic;
using System;

namespace Models
{
    public class Core : BindableBase
    {
        private static readonly Core instance = new Core();
        private FileReader storage;

        public static Core Instance
        {
            get
            {
                return instance;
            }
        }

        public FileReader Storage
        {
            get
            {
                return storage;
            }
            set
            {
                // TODO ! old code
                //// Clear Pool
                //Pool.Clear();
                //FinishedPool.Clear();
                //Routines.Clear();
                //// Set new storage and load tasks.
                storage = value;
                //if (storage != null)
                //{
                //    foreach (TDTask task in storage.GetTasks())
                //    {
                //        Pool.Add(task);
                //    }
                //    foreach (TDTask task in storage.GetFinishedTasks())
                //    {
                //        FinishedPool.Add(task);
                //    }
                //    foreach (Routine item in storage.GetRoutines())
                //    {
                //        Routines.Add(item);
                //    }
                //}
                //OnPropertyChanged(() => Pool);
                //OnPropertyChanged(() => FinishedPool);
                //OnPropertyChanged(() => Routines);
            }
        }      

        public List<string> GetTopCategories()
        {
            List<string> topCategories = new List<string>();
            // TODO stub
            topCategories.Add("Top 1");
            topCategories.Add("Top 2");
            // !
            return topCategories;
        }

        public List<string> GetSubcategories(string parent)
        {
            List<string> categories = new List<string>();
            // TODO stub
            categories.Add("sub 1");
            categories.Add("sub 2");
            // !
            return categories;
        }

        public bool AddCategory(string name, string parent)
        {
            // TODO stub
            return true;
        }

        public bool DeleteCategory(string name, string parent)
        {
            // TODO stub
            return true;
        }

        //ctor
        private Core()
        {
            // TODO ! old code
            //Pool = new List<TDTask>();
            //FinishedPool = new List<TDTask>();
            //Routines = new List<Routine>();

        }
    }
}
