namespace Models
{
    public abstract class FileReader
    {
        public abstract string Extension { get; }

        public abstract bool InitializeFile(string fileName);
        public abstract bool LoadFile(string fileName);
    }
}
