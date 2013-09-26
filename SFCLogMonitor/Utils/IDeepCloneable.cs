namespace SFCLogMonitor.Utils
{
    public interface IDeepCloneable<out T>
    {
        T DeepClone();
    }
}
