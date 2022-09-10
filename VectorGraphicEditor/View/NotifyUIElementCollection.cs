using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace VectorGraphicEditor.Collections
{
    internal class NotifyUIElementCollection : UIElementCollection, INotifyCollectionChanged
    {
        public NotifyUIElementCollection(UIElement visualParent, FrameworkElement logicalParent)
            : base(visualParent, logicalParent)
        {

        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void NotifyCollectionChanged(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }

        public override int Add(UIElement element)
        {
            int result = base.Add(element);

            NotifyCollectionChanged(NotifyCollectionChangedAction.Add);

            return result;
        }
    }
}
