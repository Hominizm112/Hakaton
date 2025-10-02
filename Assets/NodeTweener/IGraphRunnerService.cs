public interface IGraphRunnerService
{
    public abstract void RegisterAnimator(string runnerName, TweenGraphRunner runner);
    public abstract void UnregisterAnimator(string runnerName);
    public abstract TweenGraphRunner GetRunner(string runnerName);
    public abstract bool TryGetRunner(string runnerName, out TweenGraphRunner runner);
}

