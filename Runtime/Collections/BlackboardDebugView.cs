using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aurora.Collections
{
    internal sealed class BlackboardDebugView
    {
        private readonly IDictionary<string, object> _dictionary;

        public BlackboardDebugView(Blackboard blackboard)
        {
            if (blackboard == null)
            {
                throw new ArgumentNullException(nameof(blackboard));
            }
            _dictionary = blackboard.Dictionary;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, object>[] Items
        {
            get
            {
                var items = new KeyValuePair<string, object>[_dictionary.Count];
                _dictionary.CopyTo(items, 0);
                return items;
            }
        }
    }
}
