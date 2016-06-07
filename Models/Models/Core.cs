using Prism.Mvvm;

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
