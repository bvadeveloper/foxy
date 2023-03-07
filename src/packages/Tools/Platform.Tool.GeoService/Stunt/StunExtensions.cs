namespace Platform.Tool.GeoService.Stunt
{
    public static class StunExtensions
    {
        internal static async Task<T> AwaitWithTimeout<T>(this Task<T> task, int timeoutMs)
        {
            await Task.WhenAny(task, Task.Delay(timeoutMs));

            return task.IsCompleted
                ? await task
                : throw new Exception("Task timeout");
        }

        internal static IEnumerable<T> RandomElements<T>(this T[] elements)
        {
            var random = new Random();
            yield return elements[random.Next(elements.Length)];
        }
    }
}