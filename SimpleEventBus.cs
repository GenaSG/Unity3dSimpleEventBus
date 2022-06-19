namespace SimpleEventBus{
	using System;
	using System.Collections.Generic;

	public static class EventBus<TPayload>
	{
		private static readonly object global = new object();
		private static readonly Dictionary<object,EventContainer<TPayload>> listeners = new Dictionary<object,EventContainer<TPayload>>();
		
		private class EventContainer<TPayload> 
		{
			private Action<object,TPayload>	action = delegate {};
			private int count;
			public bool IsEmpty { get { return count == 1;} }
			
			public void AddListener(Action<object, TPayload> listener)
			{
				action += listener;
				count = action.GetInvocationList().Length;
			}
			
			public void RemoveListener(Action<object, TPayload> listener)
			{
				action -= listener;
				count = action.GetInvocationList().Length;
			}
			
			public void Raise(object caller, TPayload payload)
			{
				action(caller, payload);
			}
		}
		

		public static void Raise(object caller, TPayload payload, object target)
		{
			EventContainer<TPayload> container;
			if(target == null) target = global;
			if(listeners.TryGetValue(target, out container))
			{
				container.Raise(caller,payload);	
			}
		}

		public static void Subscribe(Action<object, TPayload> listener, object target)
		{
			if(target == null) target = global;
			if(!listeners.ContainsKey(target)) 
				listeners.Add(target,new EventContainer<TPayload>());
			listeners[target].AddListener(listener);
		}

		public static void UnSubscribe(Action<object, TPayload> listener, object target)
		{
			
			if(target == null) target = global;
			EventContainer<TPayload> container;
			if(listeners.TryGetValue(target, out container))
			{
				container.RemoveListener(listener);
				if(container.IsEmpty)
				{
					listeners.Remove(target);	
				}
			}
		}
	}
	
	public class Signal<TPayload>
	{
		private static readonly object global = new object();
		private static readonly Dictionary<object,EventContainer<TPayload>> listeners = new Dictionary<object,EventContainer<TPayload>>();
		
		private class EventContainer<TPayload> 
		{
			private Action<object,TPayload>	action = delegate {};
			private int count;
			public bool IsEmpty { get { return count == 1;} }
			
			public void AddListener(Action<object, TPayload> listener)
			{
				action += listener;
				count = action.GetInvocationList().Length;
			}
			
			public void RemoveListener(Action<object, TPayload> listener)
			{
				action -= listener;
				count = action.GetInvocationList().Length;
			}
			
			public void Raise(object caller, TPayload payload)
			{
				action(caller, payload);
			}
		}
		

		public void Raise(object caller, TPayload payload, object target)
		{
			EventContainer<TPayload> container;
			if(target == null) target = global;
			if(listeners.TryGetValue(target, out container))
			{
				container.Raise(caller,payload);	
			}
		}

		public void Subscribe(Action<object, TPayload> listener, object target)
		{
			if(target == null) target = global;
			if(!listeners.ContainsKey(target)) 
				listeners.Add(target,new EventContainer<TPayload>());
			listeners[target].AddListener(listener);
		}

		public void UnSubscribe(Action<object, TPayload> listener, object target)
		{
			
			if(target == null) target = global;
			EventContainer<TPayload> container;
			if(listeners.TryGetValue(target, out container))
			{
				container.RemoveListener(listener);
				if(container.IsEmpty)
				{
					listeners.Remove(target);	
				}
			}
		}
	}

}
