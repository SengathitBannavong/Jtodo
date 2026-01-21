using System.Windows;
using System.Windows.Controls;
using Jtodo.Domains;
using Jtodo.ViewModels;
using Jtodo.DTOs;

namespace Jtodo.Selectors
{
    public class TodoListGridTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? NewTodoListTemplate { get; set; }
        public DataTemplate? TodoListTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is NewTodoListPlaceholder)
            {
                return NewTodoListTemplate;
            }
            else if (item is TodoListDto)
            {
                return TodoListTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
