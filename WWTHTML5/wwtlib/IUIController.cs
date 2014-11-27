using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;


namespace wwtlib
{
    public interface IUiController
    {
        void Render(RenderContext renderContext);
        bool MouseDown(object sender, ElementEvent e);
        bool MouseUp(object sender, ElementEvent e);
        bool MouseMove(object sender, ElementEvent e);
        bool MouseClick(object sender, ElementEvent e);
        bool Click(object sender, ElementEvent e);
        bool MouseDoubleClick(object sender, ElementEvent e);
        bool KeyDown(object sender, ElementEvent e);
        bool KeyUp(object sender, ElementEvent e);
        bool Hover(Vector2d pnt);
    }
}
