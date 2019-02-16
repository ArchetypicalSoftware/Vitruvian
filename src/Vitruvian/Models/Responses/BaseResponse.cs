using System;

namespace Archetypical.Software.Vitruvian.Models.Responses
{
    public abstract class BaseResponse
    {
        protected abstract Command Command { get; }
        public DateTimeOffset DateTimeOffset { get; } = DateTimeOffset.Now;
        public string ExecutingMachine { get; } = Environment.MachineName;
        public int ThreadId { get; } = Environment.CurrentManagedThreadId;
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}