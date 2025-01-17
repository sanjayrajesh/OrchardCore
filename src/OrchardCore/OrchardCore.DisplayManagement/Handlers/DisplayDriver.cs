using System.Threading.Tasks;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace OrchardCore.DisplayManagement.Handlers
{
    public abstract class DisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext> :
        DisplayDriverBase,
        IDisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext>
        where TModel : class
        where TDisplayContext : BuildDisplayContext
        where TEditorContext : BuildEditorContext
        where TUpdateContext : UpdateEditorContext
    {
        protected static readonly string ModelName = typeof(TModel).Name;

        /// <summary>
        /// Returns <see langword="true"/> if the model can be handled by the current driver.
        /// </summary>
        /// <remarks>
        /// Override <see cref="CanHandleModelAsync(TModel)"/> instead if your code is asynchronous.
        /// </remarks>
        public virtual bool CanHandleModel(TModel model)
        {
            return true;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the model can be handled by the current driver.
        /// </summary>
        /// <remarks>
        /// Override <see cref="CanHandleModel(TModel)"/> instead if your code is synchronous.
        /// </remarks>
        public virtual Task<bool> CanHandleModelAsync(TModel model)
            => Task.FromResult(CanHandleModel(model));

        async Task<IDisplayResult> IDisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext>.BuildDisplayAsync(TModel model, TDisplayContext context)
        {
            if (!await CanHandleModelAsync(model))
            {
                return null;
            }

            BuildPrefix(model, context.HtmlFieldPrefix);

            return await DisplayAsync(model, context);
        }

        async Task<IDisplayResult> IDisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext>.BuildEditorAsync(TModel model, TEditorContext context)
        {
            if (!await CanHandleModelAsync(model))
            {
                return null;
            }

            BuildPrefix(model, context.HtmlFieldPrefix);

            return await EditAsync(model, context);
        }

        async Task<IDisplayResult> IDisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext>.UpdateEditorAsync(TModel model, TUpdateContext context)
        {
            if (!await CanHandleModelAsync(model))
            {
                return null;
            }

            BuildPrefix(model, context.HtmlFieldPrefix);

            return await UpdateAsync(model, context);
        }

        public virtual Task<IDisplayResult> DisplayAsync(TModel model, TDisplayContext context)
        {
            return DisplayAsync(model, context.Updater);
        }

        public virtual Task<IDisplayResult> DisplayAsync(TModel model, IUpdateModel updater)
        {
            return Task.FromResult(Display(model, updater));
        }

        public virtual IDisplayResult Display(TModel model, IUpdateModel updater)
        {
            return Display(model);
        }

        public virtual IDisplayResult Display(TModel model)
        {
            return NullShapeResult();
        }

        public virtual Task<IDisplayResult> EditAsync(TModel model, TEditorContext context)
        {
            return EditAsync(model, context.Updater);
        }

        public virtual Task<IDisplayResult> EditAsync(TModel model, IUpdateModel updater)
        {
            return Task.FromResult(Edit(model, updater));
        }

        public virtual IDisplayResult Edit(TModel model, IUpdateModel updater)
        {
            return Edit(model);
        }

        public virtual IDisplayResult Edit(TModel model, TEditorContext context)
        {
            return Edit(model, context.Updater);
        }

        public virtual IDisplayResult Edit(TModel model)
        {
            return NullShapeResult();
        }

        private static IDisplayResult NullShapeResult()
        {
            return null;
        }

        public virtual Task<IDisplayResult> UpdateAsync(TModel model, TUpdateContext context)
        {
            return UpdateAsync(model, context.Updater);
        }

        public virtual Task<IDisplayResult> UpdateAsync(TModel model, IUpdateModel updater)
        {
            return EditAsync(model, updater);
        }

        protected virtual void BuildPrefix(TModel model, string htmlFieldPrefix)
        {
            if (!string.IsNullOrEmpty(htmlFieldPrefix))
            {
                Prefix = $"{htmlFieldPrefix}.{ModelName}";
            }
            else
            {
                Prefix = ModelName;
            }
        }
    }

    public abstract class DisplayDriver<TModel> :
        DisplayDriver<TModel, BuildDisplayContext, BuildEditorContext, UpdateEditorContext>,
        IDisplayDriver<TModel>
        where TModel : class
    {
    }

    public abstract class DisplayDriver<TModel, TConcrete, TDisplayContext, TEditorContext, TUpdateContext> :
        DisplayDriver<TConcrete, TDisplayContext, TEditorContext, TUpdateContext>,
        IDisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext>
        where TConcrete : class, TModel
        where TModel : class
        where TDisplayContext : BuildDisplayContext
        where TEditorContext : BuildEditorContext
        where TUpdateContext : UpdateEditorContext
    {
        /// <summary>
        /// Returns <c>true</c> if the model can be handle by the current driver.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanHandleModel(TModel model)
        {
            return true;
        }

        Task<IDisplayResult> IDisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext>.BuildDisplayAsync(TModel model, TDisplayContext context)
        {
            var concrete = model as TConcrete;

            if (concrete == null || !CanHandleModel(concrete))
            {
                return Task.FromResult<IDisplayResult>(null);
            }

            BuildPrefix(concrete, context.HtmlFieldPrefix);

            return DisplayAsync(concrete, context);
        }

        Task<IDisplayResult> IDisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext>.BuildEditorAsync(TModel model, TEditorContext context)
        {
            var concrete = model as TConcrete;

            if (concrete == null || !CanHandleModel(concrete))
            {
                return Task.FromResult<IDisplayResult>(null);
            }

            BuildPrefix(concrete, context.HtmlFieldPrefix);

            return EditAsync(concrete, context);
        }

        Task<IDisplayResult> IDisplayDriver<TModel, TDisplayContext, TEditorContext, TUpdateContext>.UpdateEditorAsync(TModel model, TUpdateContext context)
        {
            var concrete = model as TConcrete;

            if (concrete == null || !CanHandleModel(concrete))
            {
                return Task.FromResult<IDisplayResult>(null);
            }

            BuildPrefix(concrete, context.HtmlFieldPrefix);

            return UpdateAsync(concrete, context);
        }
    }

    public abstract class DisplayDriver<TModel, TConcrete> :
        DisplayDriver<TModel, TConcrete, BuildDisplayContext, BuildEditorContext, UpdateEditorContext>,
        IDisplayDriver<TModel>
        where TConcrete : class, TModel
        where TModel : class
    {
    }
}
