using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Innovation
{
    // A custom collection for managing components in a GameScreen
    public class ComponentCollection : Collection<Component>
    {
        GameScreen owner;

        // The list containing each component's index in the
        // component list, in the order we want them to draw
        List<int> inDrawOrder = new List<int>();

        public ComponentCollection(GameScreen owner)
        {
            this.owner = owner;
        }

        // Override InsertItem so we can set the parent of the component
        protected override void InsertItem(int index, Component item)
        {
            if (item.Parent != null && item.Parent != owner)
            {
                item.Parent.Components.Remove(item);
            }

            item.Parent = owner;

            // Tell what to do when the item's draw order changes
            item.DrawOrderChanged += new ComponentDrawOrderChangedEventHandler(ComponentDrawOrderChangeEventHandler);

            base.InsertItem(index, item);

            UpdateDrawPosition(item);
        }

        // Draw order changed event handler
        void ComponentDrawOrderChangeEventHandler(object sender, ComponentDrawOrderChangedEventArgs e)
        {
            UpdateDrawPosition(e.Component);
        }

        // Updates the position of the component in the draw order list
        void UpdateDrawPosition(Component Component)
        {
            int ord = Component.DrawOrder;
            int loc = Items.IndexOf(Component);

            if (inDrawOrder.Contains(loc))
                inDrawOrder.Remove(loc);

            int i = 0;

            // Search ordered list until we find a component of lesser or equal draw order value
            if (ord > 0)
            {
                while (i < inDrawOrder.Count)
                {
                    // If current item's draw order is greater or equal to the one we're working with...
                    if (Items[inDrawOrder[i]].DrawOrder >= ord)
                    {
                        // If it is greater, decrement it so it is above the component we are moving's draw order...
                        if (Items[inDrawOrder[i]].DrawOrder > ord)
                            --i;

                        break;
                    }
                    else
                    {
                        ++i;
                    }
                }
            }

            inDrawOrder.Insert(i, Items.IndexOf(Component));
        }

        // Tells what enumerator to use when we want to loop through the components by draw order
        public ComponentEnumerator InDrawOrder
        {
            get { return new ComponentEnumerator(this, inDrawOrder); }
        }

        // Override RemoveItem so we can set the parent of the component to null
        protected override void RemoveItem(int index)
        {
            Items[index].Parent = null;

            // Unhook draw order change event
            Items[index].DrawOrderChanged -= ComponentDrawOrderChangeEventHandler;

            base.RemoveItem(index);

            inDrawOrder.Clear();
            foreach (Component component in Items)
                UpdateDrawPosition(component);
        }
    }
}