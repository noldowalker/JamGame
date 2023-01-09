using System;
using System.Collections.Generic;

namespace GameLogic
{
    public static class ObserverWithoutData
    {
        public delegate void EventHandler();
        private static Dictionary<Enum, EventHandler> EventHandlers;

        static ObserverWithoutData()
        {
            EventHandlers = new Dictionary<Enum, EventHandler>();
        }

        public static void Clear()
        {
            EventHandlers.Clear();
        }

        public static void FireEvent(Enum eventType)
        {
            if (!EventHandlers.ContainsKey(eventType)) 
                return;
            
            EventHandlers[eventType]?.Invoke();
        }

        public static void Sub(Enum eventType, EventHandler @delegate)
        {
            if (@delegate == null)
                return;
                    
            if (!EventHandlers.ContainsKey(eventType))
                EventHandlers[eventType] = null;

            EventHandlers[eventType] += @delegate;
        }

        public static void Unsub(Enum eventType, EventHandler @delegate)
        {
            if (!EventHandlers.ContainsKey(eventType)) 
                return;
            
            if (@delegate != null)
            {
                EventHandlers[eventType] -= @delegate;
            }
        }
    }

    public static class ObserverWithData<T> where T : ObservingDataTransferObject
    {
        public delegate void EventHandler(T dto);

        private static Dictionary<Enum, EventHandler> EventHandlers;

        static ObserverWithData()
        {
            EventHandlers = new Dictionary<Enum, EventHandler>();
        }

        public static void Clear()
        {
            EventHandlers.Clear();
        }

        public static void FireEvent(Enum eventType, T message)
        {
            if (!EventHandlers.ContainsKey(eventType)) 
                return;
            
            EventHandlers[eventType]?.Invoke(message);
        }

        public static void Sub(Enum eventType, EventHandler handler)
        {
            if (handler == null)
                return;
                    
            if (!EventHandlers.ContainsKey(eventType))
                EventHandlers[eventType] = null;
                
            EventHandlers[eventType] += handler;
            
        }

        public static void Unsub(Enum eventType, EventHandler handler)
        {
            if (!EventHandlers.ContainsKey(eventType)) 
                return;
            
            if (handler != null)
            {
                EventHandlers[eventType] -= handler;
            }
        }
    }
}