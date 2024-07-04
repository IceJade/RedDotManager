
// 链表连接器，双向循环链表
public class ListLinker
{
    public struct Iterator
    {
        private ILinkedNode m_start;
        private ILinkedNode m_current;
        private ILinkedNode m_next;

        public ILinkedNode Current { get { return m_current; } }

        public Iterator(ILinkedNode node)
        {
            m_start = node;
            m_current = null;
            m_next = node;
        }

        public bool MoveNext()
        {
            m_current = m_next;
            if (m_next != null && m_start != null && m_next.Next != m_start && m_next.Next != m_current)
            {
                m_next = m_next.Next;
            }
            else
            {
                m_next = null;
            }
            return m_current != null;
        }

        public void RemoveCurrent()
        {
            if (m_current != null)
            {
                if (m_current == m_start)
                {
                    if (m_current.Next != m_start)
                    {
                        m_start = m_current.Next;
                    }
                    else
                    {
                        m_start = null;
                    }
                }
                if (m_current == m_next)
                {
                    m_next = null;
                }
                ListLinker.Remove(m_current);
            }
        }

        public ILinkedNode GetStart()
        {
            return m_start;
        }
    }

    public static ILinkedNode AddPrevious(ILinkedNode node, ILinkedNode newPreviousNode)
    {
        if (node != null && newPreviousNode != null && newPreviousNode.Previous == null && newPreviousNode.Next == null)
        {
            ILinkedNode previousNode = node.Previous;
            node.Previous = newPreviousNode;
            newPreviousNode.Next = node;
            if (previousNode != null)
            {
                newPreviousNode.Previous = previousNode;
                previousNode.Next = newPreviousNode;
            }
            else
            {
                newPreviousNode.Previous = node;
                node.Next = newPreviousNode;
            }
            return newPreviousNode;
        }
        return null;
    }

    public static ILinkedNode AddNext(ILinkedNode node, ILinkedNode newNextNode)
    {
        if (node != null && newNextNode != null && newNextNode.Previous == null && newNextNode.Next == null)
        {
            ILinkedNode nextNode = node.Next;
            node.Next = newNextNode;
            newNextNode.Previous = node;
            if (nextNode != null)
            {
                newNextNode.Next = nextNode;
                nextNode.Previous = newNextNode;
            }
            else
            {
                node.Previous = newNextNode;
                newNextNode.Next = node;
            }
            return newNextNode;
        }
        return null;
    }

    public static ILinkedNode Remove(ILinkedNode node)
    {
        if (node != null)
        {
            ILinkedNode previousNode = node.Previous;
            ILinkedNode nextNode = node.Next;
            if (previousNode != null)
            {
                previousNode.Next = nextNode;
            }
            if (nextNode != null)
            {
                nextNode.Previous = previousNode;
            }
            node.Previous = null;
            node.Next = null;
            return nextNode != node ? nextNode : null;
        }
        return null;
    }
}
