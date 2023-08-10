using System;
using System.Collections.Generic;
using UnityEngine;

namespace HAVIGAME {
    public class EventArgs : IReferencePoolable {
        public object Sender { get; private set; }

        public EventArgs() {
            Sender = null;
        }

        public virtual void Clear() {
            Sender = null;
        }

        public static EventArgs Create(object sender = null) {
            EventArgs eventArgs = ReferencePool.Acquire<EventArgs>();
            eventArgs.Sender = sender;
            return eventArgs;
        }

        public static T Create<T>(object sender = null) where T : EventArgs, new() {
            T eventArgs = ReferencePool.Acquire<T>();
            eventArgs.Sender = sender;
            return eventArgs;
        }
    }

    public class Dispatcher<T> {

        private class Listener {

            private readonly List<Action<EventArgs>> listeners;
            private readonly List<Action<EventArgs>> cached;

            public Listener(int capacity) {
                this.listeners = new List<Action<EventArgs>>();
                this.cached = new List<Action<EventArgs>>();
            }

            public void AddListener(Action<EventArgs> callback) {
                listeners.Add(callback);
            }
            public void RemoveListener(Action<EventArgs> callback) {
                listeners.Remove(callback);
            }
            public void Dispatch(EventArgs eventArgs) {
                cached.Clear();

                cached.AddRange(listeners);

                foreach (Action<EventArgs> action in cached) {
                    action?.Invoke(eventArgs);
                }
            }
        }

        private readonly Dictionary<T, Listener> listeners;

        public Dispatcher(int capacity) {
            listeners = new Dictionary<T, Listener>(capacity);
        }

        public void AddListener(T key, Action<EventArgs> callback) {
            Listener listener;
            if (listeners.TryGetValue(key, out listener)) {
                listener.AddListener(callback);
            }
            else {
                listener = new Listener(4);
                listener.AddListener(callback);
                listeners.Add(key, listener);
            }
        }

        public void RemoveListener(T key, Action<EventArgs> callback) {
            Listener listener;
            if (listeners.TryGetValue(key, out listener)) {
                listener.RemoveListener(callback);
            }
        }

        public void Dispatch(T key, EventArgs eventArgs) {
            Listener listener;
            if (listeners.TryGetValue(key, out listener)) {
                listener.Dispatch(eventArgs);
            }

            ReferencePool.Release(eventArgs);
        }
    }

    public static class EventDispatcher {
        public static readonly InitializeEvent initializeEvent = new InitializeEvent();

        private const int initializeCapacity = 64;
        private static Dispatcher<int> dispatcher;

        public static bool IsInitialized => initializeEvent.IsInitialized;

        public static void Initialize() {
            if (initializeEvent.IsRunning) {
                Log.Warning("[EventDispatcher] EventDispatcher is running with initialize state {0}", IsInitialized);
                return;
            }

            dispatcher = new Dispatcher<int>(initializeCapacity);

            Log.Info("[EventDispatcher] Initialize completed.");
            initializeEvent.Invoke(true);
        }

        /// <summary>
        /// Add an event listener.
        /// </summary>
        /// <param name="eventID"> Unique event ID. </param>
        /// <param name="callback"> Event callback. </param>
        public static void AddListener(int eventID, Action<EventArgs> callback) {
            dispatcher.AddListener(eventID, callback);
        }

        /// <summary>
        /// Remove an event listener.
        /// </summary>
        /// <param name="eventID"> Unique event ID. </param>
        /// <param name="callback"> Event callback. </param>
        public static void RemoveListener(int eventID, Action<EventArgs> callback) {
            dispatcher.RemoveListener(eventID, callback);
        }

        /// <summary>
        /// Broadcast an event.
        /// </summary>
        /// <param name="eventID"> Unique event ID. </param>
        /// <param name="eventArgs"> Event paramater. </param>
        public static void Dispatch(int eventID, EventArgs eventArgs) {
            dispatcher.Dispatch(eventID, eventArgs);
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModule() {
            GameManager.RegisterModule<Initializer>();
        }

        public class Initializer : ModuleInitializer {

            public override int Order => CORE_MODULE;
            public override InitializeEvent InitializeEvent => EventDispatcher.initializeEvent;

            public override void Initialize() {
                EventDispatcher.Initialize();
            }
        }
    }
}
