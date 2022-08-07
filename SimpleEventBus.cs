using UnityEngine;

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
		public void Raise(object caller, TPayload payload, object target)
		{
			EventBus<TPayload>.Raise(caller,payload,target);
		}

		public void Subscribe(Action<object, TPayload> listener, object target)
		{
			EventBus<TPayload>.Subscribe(listener,target);
		}

		public void UnSubscribe(Action<object, TPayload> listener, object target)
		{
			EventBus<TPayload>.UnSubscribe(listener,target);
		}
	}
	

	public interface IEventHandler{}

	public static class InterfaceEventBus<TPayload> where TPayload : IEventHandler
	{
		private static readonly object global = new object();
		private static readonly Dictionary<object,HashSet<TPayload>> listeners = new Dictionary<object,HashSet<TPayload>>();

		public static void Raise(Action<TPayload> handler, object target)
		{
			HashSet<TPayload> container;

			if(target == null) target = global;
			if(listeners.TryGetValue(target, out container))
			{
				foreach (TPayload c in container)
				{
					handler(c);
				}
			}
		}

		public static void Subscribe(TPayload listener, object target)
		{
			if(target == null) target = global;
			if(!listeners.ContainsKey(target))
				listeners.Add(target,new HashSet<TPayload>());
			listeners[target].Add(listener);
		}

		public static void UnSubscribe(TPayload listener, object target)
		{

			if(target == null) target = global;
			HashSet<TPayload> container;
			if(listeners.TryGetValue(target, out container))
			{
				container.Remove(listener);
				if(container.Count == 0)
				{
					listeners.Remove(target);
				}
			}
		}
	}

	public class InterfaceSignal<TPayload> where TPayload : IEventHandler
	{
		public void Raise(Action<TPayload> handler, object target)
		{
			InterfaceEventBus<TPayload>.Raise(handler,target);
		}

		public void Subscribe(TPayload listener, object target)
		{
			InterfaceEventBus<TPayload>.Subscribe(listener,target);
		}

		public void UnSubscribe(TPayload listener, object target)
		{
			InterfaceEventBus<TPayload>.UnSubscribe(listener,target);
		}
	}

}
