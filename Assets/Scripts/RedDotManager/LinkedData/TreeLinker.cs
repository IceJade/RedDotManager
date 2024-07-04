
// 链表连接器，树形链表
public class TreeLinker
{
    public struct Iterator
    {
        private ILinkedTreeNode m_start;
        private ILinkedTreeNode m_current;
        private ILinkedTreeNode m_next;

        public ILinkedTreeNode Current { get { return m_current; } }

        public Iterator(ILinkedTreeNode node)
        {
            m_start = node;
            m_current = null;
            m_next = node;
        }

        public bool MoveNext()
        {
            m_current = m_next;
            if (m_next != null)
            {
                if (m_next.Child != null)
                {
                    m_next = m_next.Child;
                }
                else if (HasNext(m_next))
                {
                    m_next = m_next.Next as ILinkedTreeNode;
                }
                else if (HasNext(m_next.Parent))
                {
                    m_next = m_next.Parent.Next as ILinkedTreeNode;
                }
                else
                {
                    m_next = null;
                }
            }
            return m_current != null;
        }

        private bool HasNext(ILinkedTreeNode node)
        {
            return node != null && node.Next != null && ((node.Parent != null && node.Next != node.Parent.Child) || (node.Parent == null && node.Next != m_start));
        }
    }

    public static ILinkedTreeNode AddPrevious(ILinkedTreeNode node, ILinkedTreeNode newPreviousNode)
    {
        if (ListLinker.AddPrevious(node, newPreviousNode) != null)
        {
            ILinkedTreeNode parentNode = node.Parent;
            if (parentNode != null)
            {
                newPreviousNode.Parent = parentNode;
                if (parentNode.Child == node)
                {
                    parentNode.Child = newPreviousNode;
                }
            }
            return newPreviousNode;
        }
        return null;
    }

    public static ILinkedTreeNode AddNext(ILinkedTreeNode node, ILinkedTreeNode newNextNode)
    {
        if (ListLinker.AddNext(node, newNextNode) != null)
        {
            newNextNode.Parent = node.Parent;
            return newNextNode;
        }
        return null;
    }

    public static ILinkedTreeNode AddParent(ILinkedTreeNode node, ILinkedTreeNode newParentNode)
    {
        if (node != null && newParentNode != null && newParentNode.Previous == null && newParentNode.Next == null)
        {
            ILinkedTreeNode oldParentNode = node.Parent;
            if (oldParentNode != null)
            {
                ListLinker.Iterator iter = new ListLinker.Iterator(oldParentNode.Child);
                while (iter.MoveNext())
                {
                    if (iter.Current is ILinkedTreeNode)
                    {
                        (iter.Current as ILinkedTreeNode).Parent = newParentNode;
                    }
                }
                newParentNode.Child = oldParentNode.Child;
                oldParentNode.Child = newParentNode;
                newParentNode.Parent = oldParentNode;
            }
            else
            {
                newParentNode.Child = node;
                node.Parent = newParentNode;
            }
            return newParentNode;
        }
        return null;
    }

    public static ILinkedTreeNode AddChild(ILinkedTreeNode node, ILinkedTreeNode newChildNode)
    {
        if (node != null && newChildNode != null && newChildNode.Previous == null && newChildNode.Next == null)
        {
            newChildNode.Parent = node;
            if (node.Child != null)
            {
                if (node.Child.Previous == null)
                {
                    ListLinker.AddNext(node.Child, newChildNode);
                }
                else
                {
                    ListLinker.AddNext(node.Child.Previous, newChildNode);
                }
            }
            else
            {
                node.Child = newChildNode;
            }
            return newChildNode;
        }
        return null;
    }

    public static ILinkedTreeNode Remove(ILinkedTreeNode node)
    {
        if (node != null)
        {
            ILinkedTreeNode parentNode = node.Parent;
            ILinkedTreeNode nextNode = ListLinker.Remove(node) as ILinkedTreeNode;
            if (parentNode.Child == node)
            {
                parentNode.Child = node != nextNode ? nextNode : null;
            }
            ListLinker.Iterator iter = new ListLinker.Iterator(node);
            while (iter.MoveNext())
            {
                if (iter.Current is ILinkedTreeNode)
                {
                    (iter.Current as ILinkedTreeNode).Parent = null;
                }
            }
            return nextNode;
        }
        return null;
    }

    #region Parser

    private const char SYMBOL_SPLIT = ',';
    private const char SYMBOL_PARENTHESIS_L = '[';
    private const char SYMBOL_PARENTHESIS_R = ']';
    
    public delegate T CreateTreeNodeDelegate<T>(string data, int startIndex, int endIndex) where T : ILinkedTreeNode;

    public T Parse<T>(string data, CreateTreeNodeDelegate<T> create) where T : class, ILinkedTreeNode
    {
        if (string.IsNullOrEmpty(data) || create == null)
        {
            return null;
        }
        T root = null;
        T current = null;
        T temp;
        int length = data.Length;
        char c;
        int startIndex = -1;
        bool isAddChild = false;
        for (int i = 0; i < length; ++i)
        {
            c = data[i];
            switch (c)
            {
                case SYMBOL_SPLIT:
                    if (startIndex >= 0)
                    {
                        temp = create(data, startIndex, i - 1);
                        if (current != null)
                        {
                            if (isAddChild)
                            {
                                isAddChild = false;
                                current = AddChild(current, temp) as T;
                            }
                            else
                            {
                                current = AddNext(current, temp) as T;
                            }
                        }
                        else
                        {
                            current = temp;
                        }
                        temp = null;
                        if (root == null)
                        {
                            root = current;
                        }
                        startIndex = -1;
                    }
                    break;
                case SYMBOL_PARENTHESIS_L:
                    isAddChild = true;
                    break;
                case SYMBOL_PARENTHESIS_R:
                    if (current != null)
                    {
                        current = current.Parent as T;
                    }
                    break;
                default:
                    if (startIndex < 0)
                    {
                        startIndex = i;
                    }
                    break;
            }
        }
        return root;
    }

    #endregion
}
