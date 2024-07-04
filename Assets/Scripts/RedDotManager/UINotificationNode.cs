using System.Collections.Generic;

namespace Tetris
{
    // 界面通知节点
    public class UINotificationNode : ILinkedTreeNode
    {
        //----------------------------------------------------------------------
        // 常量

        protected const int NOTIFY_PARENT_COUNT = 1;
        protected const int NOTIFY_PARENT_ERASE = 2;

        //----------------------------------------------------------------------
        // 成员变量

        protected RedDotManager m_manager = null;
        protected string m_path = "";
        protected int m_nodeHash = 0;
        protected HashSet<int> m_selfCallerSet = null;
        protected int m_selfNotificationCount = 0;
        protected int m_childNotificationCount = 0;
        protected int m_lastSetCount = 0;
        protected bool m_alwaysHide = false;
        protected bool m_redDotVisible = false;
        protected long m_currentSerialNum = 0;
        protected long m_displaySerialNum = 0;

#if UNITY_EDITOR
        protected HashSet<string> m_debugCallerSet = null;
#endif


        //----------------------------------------------------------------------
        // 属性

        public ILinkedTreeNode Parent { get; set; }
        public ILinkedTreeNode Child { get; set; }
        public ILinkedNode Previous { get; set; }
        public ILinkedNode Next { get; set; }

        public string Path { get { return m_path; } }
        public int NodeHash { get { return m_nodeHash; } }
        public int NotifyParent = NOTIFY_PARENT_COUNT;
        public int NotificationCount { get { return m_selfNotificationCount + m_childNotificationCount; } }
        public bool IsRedDotVisible { get { return NotificationCount > 0 && m_currentSerialNum < m_displaySerialNum && !m_alwaysHide; } }

        //----------------------------------------------------------------------
        // 构造函数

        public UINotificationNode(RedDotManager manager, string path)
        {
            m_manager = manager;
            m_path = path;
            m_nodeHash = RedDotManager.GetNodeHash(m_path);
        }

        //----------------------------------------------------------------------
        // 公用函数

        public void Reset()
        {
            m_redDotVisible = false;
            m_currentSerialNum = 0;
            m_displaySerialNum = 0;
        }

        public bool IsRoot()
        {
            return Parent == null;
        }

        public void AddChild(UINotificationNode node)
        {
            if (node == null) return;
            TreeLinker.AddChild(this, node);
        }

        public void IncreaseNotificationCount(string caller, bool sendEvent = true)
        {
            UpdateDisplaySerialNum();
            int oldCount = NotificationCount;
            int callerId = RedDotManager.GetNodeHash(caller);
            if (m_selfCallerSet == null) m_selfCallerSet = new HashSet<int>();
            else if (m_selfCallerSet.Contains(callerId)) return;
            m_selfCallerSet.Add(callerId);
#if UNITY_EDITOR
            if (m_debugCallerSet == null) m_debugCallerSet = new HashSet<string>();
            m_debugCallerSet.Add(caller);
#endif
            m_selfNotificationCount = m_selfCallerSet.Count;
            if (NotifyParent > 0)
            {
                UpdateParentNotificationCount(sendEvent);
            }
            if (sendEvent)
            {
                UpdateRedDot();
            }
        }

        public void DecreaseNotificationCount(string caller, bool sendEvent = true)
        {
            if (m_selfCallerSet != null)
            {
                int callerId = RedDotManager.GetNodeHash(caller);
                if (m_selfCallerSet.Remove(callerId))
                {
#if UNITY_EDITOR
                    if (m_debugCallerSet != null) m_debugCallerSet.Remove(caller);
#endif
                    m_selfNotificationCount = m_selfCallerSet.Count;
                    UpdateParentNotificationCount(sendEvent);
                    if (sendEvent)
                    {
                        UpdateRedDot();
                    }
                }
            }
        }

        public void ClearNotificationCount(bool clearChildren, bool sendEvent = true)
        {
            if (m_selfCallerSet != null)
            {
                m_selfCallerSet.Clear();
            }
#if UNITY_EDITOR
            if (m_debugCallerSet != null)
            {
                m_debugCallerSet.Clear();
            }
#endif
            m_selfNotificationCount = 0;
            int childNotifyParent = 0;
            if (clearChildren && Child != null)
            {
                ListLinker.Iterator iter = new ListLinker.Iterator(Child);
                UINotificationNode childNode = null;
                while (iter.MoveNext())
                {
                    childNode = (iter.Current as UINotificationNode);
                    childNotifyParent |= childNode.NotifyParent;
                    childNode.ClearNotificationCount(true, sendEvent);
                }
            }
            // 如果子节点没有配置通知父级，则由此节点通知父级
            if (childNotifyParent <= 0)
            {
                UpdateParentNotificationCount(sendEvent);
            }
            if (sendEvent)
            {
                UpdateRedDot();
            }
        }

        public bool IsAlwaysHide()
        {
            return m_alwaysHide;
        }

        public void SetAlwaysHide(bool value)
        {
            if (m_alwaysHide != value)
            {
                m_alwaysHide = value;
                UpdateRedDot();
            }
        }

        public void EraseDisplay()
        {
            if (m_currentSerialNum < m_displaySerialNum)
            {
                m_currentSerialNum = m_displaySerialNum;
                UpdateRedDot();
            }
            if (Parent != null && NotifyParent > 0)
            {
                UpdateParentNotificationCount();
                if (NotifyParent == NOTIFY_PARENT_ERASE)
                {
                    UINotificationNode parentNode = (Parent as UINotificationNode);
                    parentNode.EraseDisplay();
                }
            }
        }

        public void OnModuleEnableInited()
        {
            UpdateParentNotificationCount();
            UpdateRedDot();
        }

        public void OnModuleSwitchUpdate()
        {
            OnModuleEnableInited();
        }

        //----------------------------------------------------------------------
        // 内部函数

        protected void ChangeChildNotificationCount(int count, bool sendEvent = true)
        {
            m_childNotificationCount += count;
            if (m_childNotificationCount < 0) m_childNotificationCount = 0;
            if (sendEvent)
            {
                UpdateRedDot();
            }
        }

        protected void UpdateParentNotificationCount(bool sendEvent = true)
        {
            if (Parent != null && NotifyParent > 0)
            {
                UINotificationNode parentNode = (Parent as UINotificationNode);
                int count = IsRedDotVisible ? NotificationCount : 0;
                int diff = count - m_lastSetCount;
                m_lastSetCount = count;
                if (diff != 0)
                {
                    parentNode.ChangeChildNotificationCount(diff, sendEvent);
                }
                parentNode.UpdateParentNotificationCount(sendEvent);
            }
        }

        protected void UpdateDisplaySerialNum()
        {
            m_displaySerialNum = RedDotManager.GetDisplaySerialNum();
            if (Parent != null && NotifyParent > 0)
            {
                UINotificationNode parentNode = (Parent as UINotificationNode);
                parentNode.UpdateDisplaySerialNum();
            }
        }

        protected void UpdateRedDot()
        {
            bool visible = IsRedDotVisible;
            if (m_redDotVisible != visible)
            {
                m_redDotVisible = visible;
                m_manager.OnNotification(this, m_redDotVisible);
            }
        }

#if UNITY_EDITOR

        public HashSet<string> DebugGetCallerNames()
        {
            return m_debugCallerSet;
        }

#endif
    }
}
