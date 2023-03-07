namespace Platform.Tool.GeoService.Stunt
{
    public static class StunExtensions
    {
        public static async ValueTask<T> AwaitWithTimeout<T>(this Task<T> task, int timeoutMs)
        {
            await Task.WhenAny(task, Task.Delay(timeoutMs));

            return !task.IsCompleted
                ? throw new Exception("Task timeout")
                : await task;
        }

        internal static IEnumerable<T> RandomElements<T>(this T[] elements)
        {
            var random = new Random();
            var index = random.Next(elements.Length);

            yield return elements[index];
        }
    }
}