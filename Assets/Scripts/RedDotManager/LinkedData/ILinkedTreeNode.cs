
public interface ILinkedTreeNode : ILinkedNode
{
    ILinkedTreeNode Parent { get; set; }
    ILinkedTreeNode Child { get; set; }
}