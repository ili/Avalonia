using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;

#nullable enable

namespace Avalonia.Controls.Generators
{
    public class TreeItemContainerGenerator<T> : ItemContainerGenerator<T>
        where T : class, IControl, new()
    {
        public TreeItemContainerGenerator(
            ItemsControl owner,
            AvaloniaProperty contentProperty,
            AvaloniaProperty contentTemplateProperty,
            AvaloniaProperty itemsProperty,
            AvaloniaProperty isExpandedProperty)
            : base(owner, contentProperty, contentTemplateProperty)
        {
            ItemsProperty = itemsProperty;
            IsExpandedProperty = isExpandedProperty;
        }

        /// <summary>
        /// Gets the item container's Items property.
        /// </summary>
        protected AvaloniaProperty ItemsProperty { get; }

        /// <summary>
        /// Gets the item container's IsExpanded property.
        /// </summary>
        protected AvaloniaProperty IsExpandedProperty { get; }

        protected override IControl CreateContainer(ElementFactoryGetArgs args)
        {
            if (args.Data is T c)
            {
                return c;
            }

            var result = base.CreateContainer(args);
            var template = GetTreeDataTemplate(args.Data, Owner.ItemTemplate);
            var itemsSelector = template.ItemsSelector(args.Data);

            if (itemsSelector != null)
            {
                BindingOperations.Apply(result, ItemsProperty, itemsSelector, null);
            }

            return result;
        }

        private ITreeDataTemplate GetTreeDataTemplate(object item, IDataTemplate primary)
        {
            var template = Owner.FindDataTemplate(item, primary) ?? FuncDataTemplate.Default;
            var treeTemplate = template as ITreeDataTemplate ?? new WrapperTreeDataTemplate(template);
            return treeTemplate;
        }

        class WrapperTreeDataTemplate : ITreeDataTemplate
        {
            private readonly IDataTemplate _inner;
            public WrapperTreeDataTemplate(IDataTemplate inner) => _inner = inner;
            public IControl Build(object param) => _inner.Build(param);
            public bool SupportsRecycling => _inner.SupportsRecycling;
            public bool Match(object data) => _inner.Match(data);
            public InstancedBinding ItemsSelector(object item) => null;
        }
    }
}
