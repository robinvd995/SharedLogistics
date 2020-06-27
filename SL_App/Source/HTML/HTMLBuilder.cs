using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.HTML
{
    public class HTMLBuilder
    {
        private HTMLElement _rootElement;
        private int depth = 0;

        private HTMLElement _currentElement;

        public void PushElement(string tag, bool single = false)
        {
            HTMLElement element = new HTMLElement()
            {
                ElementTag = tag,
                Single = single
            };

            if (depth > 0)
            {
                element.Parent = _currentElement;
                _currentElement.Children.Add(element);
            }
            else
            {
                _rootElement = element;
            }

            depth++;
            _currentElement = element;
        }

        public void SetValue(string value)
        {
            _currentElement.Value = value;
        }

        public void AddAttribute(string identifier, string value)
        {
            _currentElement.Attributes.Add(identifier, value);
        }

        public void PopElement()
        {
            if (depth > 0)
            {
                _currentElement = _currentElement.Parent;
                depth--;
            }
            else
            {
                Console.WriteLine("Trying to pop the root element!");
            }
        }

        public string BuildHTML()
        {
            StringBuilder builder = new StringBuilder();

            _rootElement.BuildElement(builder);

            return builder.ToString();
        }
    }
}
