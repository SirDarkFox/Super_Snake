using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Super_Snake.SnakeCore
{
    public class Apple
    {
        private UIElement _uiElement;

        public UIElement UiElement 
        { 
            get => _uiElement; 
            set => _uiElement = value; 
        }
        public Point Position { get; set; }
    }
}
