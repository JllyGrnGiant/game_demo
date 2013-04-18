using System.Collections.ObjectModel;

namespace Innovation
{
    public class GameScreenCollection : KeyedCollection<string, GameScreen>
    {
        // Allow us to get a screen by name
        protected override string GetKeyForItem(GameScreen item)
        {
            return item.Name;
        }

        protected override void RemoveItem(int index)
        {
            GameScreen screen = Items[index];

            if (Engine.DefaultScreen == screen)
                Engine.DefaultScreen = Engine.BackgroundScreen;

            base.RemoveItem(index);
        }
    }
}