using System;
using System.Collections.Generic;

public abstract class IUINotificationOP : ILinkedNode
{
    public ILinkedNode Previous { get; set; }
    public ILinkedNode Next { get; set; }
    
    public abstract void OnNotification(string path, int nodeHash, bool show);
}
