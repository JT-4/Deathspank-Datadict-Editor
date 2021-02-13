using System.Windows;
using System.Windows.Controls;

namespace Datadict_Editor
{
    public class AttributeTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// based of the "type" of datadict attribute, choose the appropriate
        /// data template defined in the mainwindow XAML
        /// </summary>
        /// <param name="item"> the bound datadict_Attribute</param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
        FrameworkElement element = container as FrameworkElement;

        DataDict_Attribute attr = item as DataDict_Attribute;
            
            //if the bound object can't be mapped to a datadict_attribute return null
            if(attr == null)
            {
                return null;
            }
            else if (attr.Type ==1 || attr.Type == 9)
            {
                //if the attribute is of type 1 or 9 use the four byte attribute data template
                return element.FindResource("FourbyteAttribute") as DataTemplate;
            }
            else if (attr.Type == 6 || attr.Type == 10)
            {
                //if the attribute is of type 6 or 10 use the eight byte attribute data template
                return element.FindResource("EightbyteAttribute") as DataTemplate;
            }
            else if (attr.Type == 12)
            {
                //if the attribute is of type 12 use the sixteen byte attribute data template
                return element.FindResource("SixteenbyteAttribute") as DataTemplate;
            }
            //else the attribute must have a value that is a string
            //use the string attribute data template
            return element.FindResource("StringAttribute") as DataTemplate;

        }
    }
}
