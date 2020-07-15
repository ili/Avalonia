using System;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace Avalonia.Controls.Generators
{
    /// <summary>
    /// Creates containers for items and maintains a list of created containers.
    /// </summary>
    /// <typeparam name="T">The type of the container.</typeparam>
    public class ItemContainerGenerator<T> : ItemContainerGenerator where T : class, IControl, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemContainerGenerator{T}"/> class.
        /// </summary>
        /// <param name="owner">The owner control.</param>
        /// <param name="contentProperty">The container's Content property.</param>
        /// <param name="contentTemplateProperty">The container's ContentTemplate property.</param>
        public ItemContainerGenerator(
            ItemsControl owner,
            AvaloniaProperty contentProperty,
            AvaloniaProperty contentTemplateProperty)
            : base(owner)
        {
            Contract.Requires<ArgumentNullException>(owner != null);
            Contract.Requires<ArgumentNullException>(contentProperty != null);

            ContentProperty = contentProperty;
            ContentTemplateProperty = contentTemplateProperty;
        }

        /// <summary>
        /// Gets the container's Content property.
        /// </summary>
        protected AvaloniaProperty ContentProperty { get; }

        /// <summary>
        /// Gets the container's ContentTemplate property.
        /// </summary>
        protected AvaloniaProperty ContentTemplateProperty { get; }

        protected override IControl CreateContainer(ElementFactoryGetArgs args)
        {
            if (args.Data is T t)
            {
                return t;
            }

            var result = new T();

            if (ContentTemplateProperty is object && Owner.ItemTemplate is object)
            {
                result.SetValue(ContentTemplateProperty, Owner.ItemTemplate, BindingPriority.Style);
            }

            result.Bind(
                ContentProperty,
                result.GetBindingObservable(Control.DataContextProperty),
                BindingPriority.Style);

            return result;
        }
    }
}
