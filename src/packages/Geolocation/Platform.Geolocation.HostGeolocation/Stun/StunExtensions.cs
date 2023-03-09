namespace Platform.Geolocation.HostGeolocation.Stun
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

        internal static IEnumerable<T> ShuffleElements<T>(this T[] elements)
        {
            var random = new Random();
            yield return elements[random.Next(elements.Length)];
        }

        internal static string ToStunUrl(this string value) => $"stun://{value}";
    }
}