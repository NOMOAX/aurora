namespace Aurora
{
    internal sealed class InvocationResult<TResult> : Invocation<TResult>
    {
        private readonly TResult _result;

        public InvocationResult(TResult result)
        {
            _result = result;
        }

        public override TResult Invoke()
        {
            return _result;
        }
    }
}
