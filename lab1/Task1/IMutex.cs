namespace lab1.Task1
{
    public interface IMutex
    {
        void Wait();
        
        void Notify();

        void NotifyAll();
    }
}