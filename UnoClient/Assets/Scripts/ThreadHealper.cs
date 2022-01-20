
namespace Unity.Threading
{
    using System;
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SystemThread = System.Threading.Thread;
    using System.Reflection;

    public delegate void Callback(params object[] args);
    //������
    internal class Listener
    {
        public string eventName;
        public string funcname;
        public object target;
        public MethodInfo targetMethod;
        public int count;
    }
    //���ݰ�(�¼�����)
    internal class EventData
    {
        public Listener listener;
        public string eventName;
        public object[] args;
    }
    //�¼�ϵͳ
    public class EventMgr
    {
        //�������б�
        private Dictionary<string, List<Listener>> events = new Dictionary<string, List<Listener>>();
        //�¼�����(�����б�)
        private LinkedList<EventData> firedDatas = new LinkedList<EventData>();
        //�����б�(���ֱ��ʹ��firedDatas�б�,��������е���ǰ,�������������¼�ϵͳ)
        private LinkedList<EventData> doingDatas = new LinkedList<EventData>();

        //��ͣ�¼�����?
        public bool pause = false;


        //ע��һ���¼�����, Ĭ�ϲ�ȡ������
        public void On(string event_name, Callback handler)
        {
            On(event_name, handler.Target, handler.Method.Name);
        }
        public void On(string event_name, Action handler)
        {
            On(event_name, handler.Target, handler.Method.Name);
        }
        public void On<T>(string event_name, Action<T> handler)
        {
            On(event_name, handler.Target, handler.Method.Name);
        }
        public void On<T1, T2>(string event_name, Action<T1, T2> handler)
        {
            On(event_name, handler.Target, handler.Method.Name);
        }
        public void On<T1, T2, T3>(string event_name, Action<T1, T2, T3> handler)
        {
            On(event_name, handler.Target, handler.Method.Name);
        }
        public void On(string event_name, object target, string funcname)
        {
            Register(events, event_name, target, funcname, -1);
        }


        //ע��һ���¼�����, ���һ�ε��ú�,ȡ������
        //�������߳���һ��once�����߷�����Ϣ, once���ܱ����ö��(������Ϣ�͵����¼�Ӱ��/�����д�������߻����޸Ĵ���)
        public void Once(string event_name, Callback handler)
        {
            Once(event_name, handler.Target, handler.Method.Name);
        }
        public void Once(string event_name, Action handler)
        {
            Once(event_name, handler.Target, handler.Method.Name);
        }
        public void Once<T>(string event_name, Action<T> handler)
        {
            Once(event_name, handler.Target, handler.Method.Name);
        }
        public void Once<T1, T2>(string event_name, Action<T1, T2> handler)
        {
            Once(event_name, handler.Target, handler.Method.Name);
        }
        public void Once<T1, T2, T3>(string event_name, Action<T1, T2, T3> handler)
        {
            Once(event_name, handler.Target, handler.Method.Name);
        }
        public void Once(string event_name, object target, string funcname)
        {
            Register(events, event_name, target, funcname, 1);
        }


        //�Ƴ��¼�����
        public void Remove(object target)
        {
            Unregister(events, target);
        }
        public void Remove(string event_name)
        {
            Unregister(events, event_name);
        }
        public void Remove(string event_name, object target, string funcname)
        {
            Unregister(events, event_name, target, funcname);
        }


        //����¼�����ؼ�¼�����
        public void Clear()
        {
            events.Clear();
            ClearDatas();
        }
        public void ClearDatas()
        {
            MonitorEnter(events);
            firedDatas.Clear();
            MonitorExit(events);

            doingDatas.Clear();
        }

        //����¼�ϵͳ,����һ������(�¼�����)
        //�������߳���һ��once�����߷�����Ϣ, once���ܱ����ö��(������Ϣ�͵����¼�Ӱ��/�����д�������߻����޸Ĵ���)
        public void Send(string event_name, params object[] args)
        {
            Fire(events, firedDatas, event_name, args);
        }


        //�¼�����(�ⲿ����)
        public void Process()
        {
            MonitorEnter(events);
            if (firedDatas.Count > 0)
            {
                var iter = firedDatas.GetEnumerator();
                while (iter.MoveNext())
                {
                    doingDatas.AddLast(iter.Current);
                }
                firedDatas.Clear();
            }
            MonitorExit(events);

            while (doingDatas.Count > 0 && !pause)
            {
                EventData data = doingDatas.First.Value;
                doingDatas.RemoveFirst();
                try
                {
                    Listener listener = data.listener;
                    listener.targetMethod.Invoke(listener.target, data.args);
                    if (listener.count > 0)
                        listener.count--;
                    if (listener.count == 0)
                    {
                        Unregister(events, listener.eventName, listener.target, listener.funcname);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Event::processOutEvents: event="
                        + data.listener.targetMethod.DeclaringType.FullName
                        + "::" + data.listener.funcname + "\n"
                        + e.ToString());
                }
            }
        }

        static void Register(Dictionary<string, List<Listener>> events, string event_name, object target, string funcname, int count)
        {
            Unregister(events, event_name, target, funcname);

            Listener listener = new Listener()
            {
                eventName = event_name,
                target = target,
                funcname = funcname,
                count = count
            };
            listener.targetMethod = target.GetType().GetMethod(funcname, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (listener.targetMethod == null)
            {
                Debug.LogError("Event::register: " + target + "not found method[" + funcname + "]");
                return;
            }

            MonitorEnter(events);
            List<Listener> lst = null;
            if (!events.TryGetValue(event_name, out lst))
            {
                lst = new List<Listener>();
                events.Add(event_name, lst);
            }
            lst.Add(listener);
            Debug.Log("Event::register: event(" + event_name + ")!");

            MonitorExit(events);
        }
        static void Unregister(Dictionary<string, List<Listener>> events, string event_name, object target, string funcname)
        {
            MonitorEnter(events);

            List<Listener> lst;
            if (!events.TryGetValue(event_name, out lst))
            {
                MonitorExit(events);
                return;
            }
            // �Ӻ���ǰ�������Ա�����;ɾ��������
            for (int i = lst.Count - 1; i >= 0; i--)
            {
                if (target == lst[i].target)
                {
                    Debug.Log("Event::deregister: event(" + lst[i].target + ":" + lst[i].funcname + ")!");
                    lst.RemoveAt(i);
                }
            }

            MonitorExit(events);
        }
        static void Unregister(Dictionary<string, List<Listener>> events, object target)
        {
            if (target == null)
            {
                Debug.LogWarning("Attempt remove NULL object  all event");
                return;
            }

            MonitorEnter(events);

            var iter = events.GetEnumerator();
            while (iter.MoveNext())
            {
                List<Listener> lst = iter.Current.Value;
                // �Ӻ���ǰ�������Ա�����;ɾ��������
                for (int i = lst.Count - 1; i >= 0; i--)
                {
                    if (target == lst[i].target)
                    {
                        Debug.Log("Event::deregister: event(" + lst[i].target + ":" + lst[i].funcname + ")!");
                        lst.RemoveAt(i);
                    }
                }
            }

            MonitorExit(events);
        }
        static void Unregister(Dictionary<string, List<Listener>> events, string event_name)
        {
            MonitorEnter(events);

            var iter = events.GetEnumerator();
            while (iter.MoveNext())
            {
                List<Listener> lst = iter.Current.Value;
                // �Ӻ���ǰ�������Ա�����;ɾ��������
                for (int i = lst.Count - 1; i >= 0; i--)
                {
                    if (event_name == lst[i].eventName)
                    {
                        Debug.Log("Event::deregister: event(" + lst[i].target + ":" + lst[i].funcname + ")!");
                        lst.RemoveAt(i);
                    }
                }
            }

            MonitorExit(events);
        }

        static void Fire(Dictionary<string, List<Listener>> events, LinkedList<EventData> cacheDatas, string event_name, params object[] args)
        {
            MonitorEnter(events);

            List<Listener> lst = null;
            if (!events.TryGetValue(event_name, out lst))
            {
                Debug.LogError("Event::fire: event(" + event_name + ") not found!");
                MonitorExit(events);
                return;
            }
            foreach (var listener in lst)
            {
                cacheDatas.AddLast(new EventData()
                {
                    listener = listener,
                    eventName = event_name,
                    args = args
                });
            }

            MonitorExit(events);
        }

        //������(��֤�̰߳�ȫ/����һ��δMonitorExitǰ, �޷��Դ˶���MonitorEnter)
        static void MonitorEnter(object obj)
        {
            Monitor.Enter(obj);
        }
        static void MonitorExit(object obj)
        {
            Monitor.Exit(obj);
        }
    }


    public class EasyThread : MonoBehaviour
    {
        //���߳�ID
        private static readonly int mainThreadId = SystemThread.CurrentThread.ManagedThreadId;

        private static EasyThread instance = null;

        public EventMgr mainRemote { get; } = new EventMgr();
        public EventMgr childRemote { get; } = new EventMgr();

        //�̶߳���/״̬
        private SystemThread thread;
        private bool runing;

        public static EasyThread GetInstance()
        {
            if (instance == null)
            {
                var obj = new GameObject("[UnityThread]");
                DontDestroyOnLoad(obj);
                instance = obj.AddComponent<EasyThread>();
            }
            return instance;
        }
        public static EasyThread NewInstance(string name = null)
        {
            var obj = new GameObject(name ?? ("[UnityThread" + UnityEngine.Random.Range(0, int.MaxValue) + "]"));
            return obj.AddComponent<EasyThread>();
        }

        void Awake()
        {
            StartThread();
        }
        void FixedUpdate()
        {
            mainRemote.Process();
        }
        void OnDestroy()
        {
            runing = false;
            mainRemote.Clear();
            childRemote.Clear();
        }

        public void StartNewThread(Thread newThread, object obj)
        {
            StopThread();

            MonitorEnter();

            runing = true;
            thread = newThread;
            thread.IsBackground = true;
            thread.Start(obj);
            
            MonitorExit();
        }

        public void StartThread()
        {
            StopThread();

            MonitorEnter();
            if (thread == null || !thread.IsAlive || !runing)
            {
                runing = true;
                thread = new SystemThread(new ThreadStart(() =>
                {
                    while (runing && this != null)
                    {
                        childRemote.Process();
                        Thread.Sleep(20);
                    }
                }));
                thread.IsBackground = true;
                thread.Start();
            }
            MonitorExit();
        }
        public void StopThread()
        {
            MonitorEnter();
            runing = false;
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
            thread = null;
            MonitorExit();
        }

        void MonitorEnter() { Monitor.Enter(this); }
        void MonitorExit() { Monitor.Exit(this); }
    }
}


